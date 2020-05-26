let smhibtn = document.getElementById("wd_source_smhi_bth");
let yrbtn = document.getElementById("wd_source_yr_btn");

getSmhiData();

smhibtn.addEventListener("click", function (e) {
    e.preventDefault();
    if (!smhibtn.classList.contains("active")) {
        getSmhiData();
        $("#weather_forecast-data_source").empty();
        smhibtn.classList.add("active");
        yrbtn.classList.remove("active");
    }
});
yrbtn.addEventListener("click", function(e) {
    e.preventDefault();
    if (!yrbtn.classList.contains("active")) {
        getYrData();
        $("#weather_forecast-data_source").html('<a href="http://www.yr.no/place/Sweden/Blekinge/Karlskrona/" target="_blank">Weather forecast from Yr, delivered by the Norwegian Meteorological Institute and the NRK</a>');
        yrbtn.classList.add("active");
        smhibtn.classList.remove("active");
    }
});

function applyCurrentWeatherForecast(lastUpdatedstr, temp, forecastDesc, windDirection, windSpeed, precipitation, airPressure, humidity, visual = null) {
    document.getElementById("weather_forecast-temp-val").innerHTML = Math.round(temp); //Temperatur
    /* Bild och beskrivning */
    $("#weather_forecast-visual").empty();
    if (visual !== null) {
        $("#weather_forecast-visual").append(visual);
    }
    $("#weather_forecast-visual").append('<p>' + forecastDesc + '</p>');

    $("#updatedForecastTime").html(lastUpdatedstr); //Uppdateringstid

    /* Räkna ut vindriktningen */
    let wdRest = windDirection % 45;
    windDirection = windDirection - wdRest;
    if (wdRest >= 22.5) {
        windDirection += 45;
    }
    $("#fc-info-wd").html(windDirections[windDirection]);

    $("#fc-info-ws").html(windSpeed);                   //Vindhastighet
    $("#fc-info-spp").html(precipitation);              //Nederbörd
    $("#fc-info-msl").html(Math.floor(airPressure));    //Lufttryck
    $("#fc-info-r").html(humidity);                     //Luftfuktighet
}

function addFutureWeatherForecast(hours, temp, forecastDesc, visual = null) {
    visualstr = "";
    if (visual !== null) {
        visualstr = '<div class="weather_forecast-future-content-item-visual">' + visual + '</div>';
    }
    $("#weather_forecast-future-content_wrap").append('<div class="weather_forecast-future-content-item"><div class="weather_forecast-future-content-item-timestamp">' + hours + ':00</div>' + visualstr + '<div class="weather_forecast-future-content-item-temp">' + Math.round(temp) + '°c</div><div class="weather_forecast-future-content-item-description">' + forecastDesc + '</div></div>');
}