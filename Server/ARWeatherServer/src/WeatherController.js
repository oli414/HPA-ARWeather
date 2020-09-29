const https = require('https');
const mysql = require('mysql');

const Weather = require("./Weather");

const settings = {
    openWeatherKey: "59c214731cf781bfed9d12a9e3cd7bfc",
    units: "metric",
    database: {
        host: "localhost",
        user: "root",
        password: "",
        database: "arweather"
    }
}

// The weather controller is responsible for handling the weather data from the database, and weather API
class WeatherController {
    constructor() {
        this.databaseConnection = mysql.createConnection(settings.database);
    }

    connectDatabase() {
        return new Promise((resolve, reject) => {
            this.databaseConnection.connect((err) => {
                if (err) {
                    console.error("Failed to connect to the database.");
                    reject(err);
                    return;
                }
                resolve();
            });
        });
    }

    // Request forecast data for a certain location
    forecastRequest(res, latitude, longitude) {
        let normalizedLatitude = Math.round(latitude * 100);
        let normalizedLongitude = Math.round(longitude * 100);
        this.databaseConnection.query(`SELECT * FROM weather_cache WHERE latitude = ${normalizedLatitude} AND longitude = ${normalizedLongitude}`, (err, results, fields) => {
            if (err) {
                res.status(500);
                res.json({
                    status: 500,
                    error: "Failed to retrieve data from the database"
                });
                throw err;
            }

            const date = new Date();
            const dateInUtc = new Date(Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds()));
            const currentEpoch = Math.round(dateInUtc.getTime() / 1000);
            const secondsInHour = 60 * 60;
            const roundedCurrentEpoch = Math.round(currentEpoch / secondsInHour) * secondsInHour;

            if (results.length > 0) {

                let firstItem = results[0];
                if (firstItem.epoch >= roundedCurrentEpoch - 60 * 60) {
                    // The cached data is up-to-date, send it directly to the user
                    const forecast = [];
                    for (let i = 0; i < results.length; i++) {
                        const weather = Weather.fromDatabaseEntry(results[i]);
                        forecast.push(weather);
                    }
                    res.json({
                        items: forecast
                    });
                    return;
                }
            }

            // The cached data is out of date or unavailble, request new data from Open Weather
            https.get(`https://api.openweathermap.org/data/2.5/onecall?units=${settings.units}&lat=${latitude}&lon=${longitude}&exclude=minutely,daily,alerts&appid=${settings.openWeatherKey}`, (resp) => {
                let data = "";

                resp.on("data", (chunk) => {
                    data += chunk;
                });

                resp.on("end", () => {
                    // Turn the JSON data into our own Weather model
                    const openWeatherData = JSON.parse(data);
                    const forecast = [];

                    const currentWeather = Weather.fromOpenWeatherData(latitude, longitude, openWeatherData.current);
                    forecast.push(currentWeather);

                    for (let i = 0; i < openWeatherData.hourly.length; i++) {
                        const hourlyWeather = Weather.fromOpenWeatherData(latitude, longitude, openWeatherData.hourly[i]);
                        forecast.push(hourlyWeather);
                    }
                    forecast.sort((a, b) => (a.epoch > b.epoch) ? 1 : -1);
                    // Send the forecast for the next 48 hours to the client
                    res.json({
                        items: forecast
                    });

                    // Check if there's data that is out-of-date that needs to be deleted
                    const insertValues = Weather.toDatabaseValues(forecast);
                    this.databaseConnection.query(`DELETE FROM weather_cache WHERE (latitude = ${normalizedLatitude} AND longitude = ${normalizedLongitude}) OR (epoch < ${roundedCurrentEpoch})`, (err) => {
                        if (err) {
                            console.error("Failed to delete rows from database");
                            throw err;
                        }

                        // Insert the newly gathered data into the database
                        this.databaseConnection.query(`INSERT INTO weather_cache(latitude, longitude, epoch, temperature, rain, precipitation) VALUES ${insertValues}`, (err) => {
                            if (err) {
                                console.error("Failed to insert rows into database");
                                throw err;
                            }
                        });
                    });
                });
            });
        });
    }
}

module.exports = WeatherController;