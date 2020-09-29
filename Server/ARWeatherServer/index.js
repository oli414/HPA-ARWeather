const express = require("express");
const app = express();
const WeatherController = require("./src/WeatherController");

const weatherController = new WeatherController();

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

weatherController.connectDatabase().then(() => {

    console.log("Connected to database.");

    app.get("/weather/:latitude/:longitude", function (req, res) {
        let latitude = req.params.latitude;
        let longitude = req.params.longitude;

        // Check input parameter validity
        if (!isNumeric(latitude) || !isNumeric(longitude)) {
            res.status(400);
            res.json({
                status: 400,
                error: "Invalid request parameters, expecting numbers"
            });
            return;
        }
        else if (latitude < -90 || latitude > 90 || longitude < -90 || longitude > 90) {
            res.status(400);
            res.json({
                status: 400,
                error: "Invalid request parameters, latitude and or longitude out of range"
            });
            return;
        }

        latitude = parseFloat(latitude).toFixed(2);
        longitude = parseFloat(longitude).toFixed(2);

        weatherController.forecastRequest(res, latitude, longitude);
    });

    app.listen(3000, () => {
        console.log("Server is now listening.");
    });

});