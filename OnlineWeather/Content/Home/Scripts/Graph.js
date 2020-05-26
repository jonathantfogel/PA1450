var form = document.getElementById("graph-filter-form");

var ctx = document.getElementById('myChart').getContext('2d');
var chartColours = { "Lufttemperatur": "rgba(255, 99, 132, 1)", "Lufttryck": "rgba(54, 162, 235, 1)", "Luftfuktighet": "rgba(255, 206, 86, 1)" };

var myChart = new Chart(ctx, {
    type: 'line',
    data: {
        labels: [],
        datasets: [{
            label: '',
            data: []
        }]
    }
});

document.getElementById("submitFormbtn").addEventListener("click", async function (e) {
    e.preventDefault();

    wd = await getWeatherData(form);

    updateGraph(myChart, wd[0].Second.map(item => item.Timestamp), wd[0].Second.map(item => item.Data), wd[0].First);
    for (let i = 1; i < wd.length; i++) {
        addLineToGraph(myChart, [], wd[i].Second.map(item => item.Data), wd[i].First);
    }
});

function updateGraph(chart, labels, data, type) {
    chart.data.labels = [];
    chart.data.datasets = [];
    addLineToGraph(chart, labels, data, type);
}

function addLineToGraph(chart, labels, data, type) {
    Array.prototype.push.apply(chart.data.labels, labels);
    chart.data.datasets.push({
        label: type,
        data: data,
        borderColor: [chartColours[type]]
    });
    chart.update();
}

async function getWeatherData(form) {
    var r = await fetch(form.action, {
        method: "POST",
        credentials: "same-origin",
        body: new FormData(form)
    });
    return await r.json();
}