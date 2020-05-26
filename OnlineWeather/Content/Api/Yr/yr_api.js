let getYrForm = document.createElement("form");
getYrForm.method = "post";
getYrForm.action = "/Home/RequestYrData";

async function getYrData() {
    var r = await fetch(getYrForm.action, {
        method: "POST",
        credentials: "same-origin",
        body: new FormData(getYrForm)
    });
    let result = await r.json();
    let forecast = result.Second;
    let updateTime = result.First;

    applyCurrentWeatherForecast(updateTime, forecast[0].temp, forecast[0].description, forecast[0].windDirection, forecast[0].windSpeed, forecast[0].precipitation, forecast[0].airPressure, "N/A");

    $("#weather_forecast-future-content_wrap").empty();
    for (var houridx = 1; houridx < 13; houridx++) {
        var timestampHours = new Date(forecast[houridx].timestamp).getHours();
        addFutureWeatherForecast(timestampHours, forecast[houridx].temp, forecast[houridx].description);
    }
}