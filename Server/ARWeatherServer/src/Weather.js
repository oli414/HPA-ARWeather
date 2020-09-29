class Weather {
    constructor(latitude, longitude, epoch, temperature, rain, precipitation) {
        this.latitude = latitude;
        this.longitude = longitude;
        this.epoch = epoch;
        this.temperature = temperature;
        this.rain = rain;
        this.precipitation = precipitation;
    }

    serialize() {
        return {
            latitude: this.latitude,
            longitude: this.longitude,
            epoch: this.epoch,
            temperature: this.temperature,
            rain: this.rain,
            precipitation: this.precipitation
        };
    }

    toDatabaseEntry() {
        return {
            latitude: Math.round(this.latitude * 100),
            longitude: Math.round(this.longitude * 100),
            epoch: this.epoch,
            temperature: Math.round(this.temperature * 100),
            rain: Math.round(this.rain * 100),
            precipitation: Math.round(this.precipitation * 100)
        };
    }

    static fromOpenWeatherData(lat, lon, weatherData) {
        let rain = 0;
        if (weatherData.rain) {
            if (weatherData.rain["1h"]) {
                rain = weatherData.rain["1h"];
            }
        }
        let pop = 0;
        if (weatherData.pop) {
            pop = weatherData.pop;
        }

        const secondsInHour = 60 * 60;
        const roundedEpoch = Math.round(weatherData.dt / secondsInHour) * secondsInHour;
        return new Weather(lat, lon, roundedEpoch, weatherData.temp, rain, pop);
    }

    static fromDatabaseEntry(data) {
        return new Weather(data.latitude / 100, data.longitude / 100, data.epoch, data.temperature / 100, data.rain / 100, data.precipitation / 100);
    }

    static toDatabaseValues(weatherList) {
        let valueSets = [];
        for (let i = 0; i < weatherList.length; i++) {
            let entry = weatherList[i].toDatabaseEntry();
            valueSets.push(`(${entry.latitude}, ${entry.longitude}, ${entry.epoch}, ${entry.temperature}, ${entry.rain}, ${entry.precipitation})`);
        }
        return valueSets.join(",");
    }
}

module.exports = Weather;