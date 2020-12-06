$(document).ready(function () {
    var date = new Date();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var year = date.getFullYear();
    var hour = date.getHours();
    var minutes = date.getMinutes();
    day = (day < 10 ? "0" : "") + day;
    month = (month < 10 ? "0" : "") + month;
    hour = (hour < 10 ? "0" : "") + hour;
    minutes = (minutes < 10 ? "0" : "") + minutes;
    var currentTime = year + "-" + month + "-" + day + "T" + hour + ":" + minutes + ":00";

    $('#endTimeChart').val(currentTime);

    var days = 7; // Days you want to subtract
    var date2 = new Date(Date.now() - (days * 24 * 60 * 60 * 1000));
    var month2 = date2.getMonth() + 1;
    var day2 = date2.getDate();
    var year2 = date2.getFullYear();
    var hour2 = date2.getHours();
    var minutes2 = date2.getMinutes();
    day2 = (day2 < 10 ? "0" : "") + day2;
    month2 = (month2 < 10 ? "0" : "") + month2;
    hour2 = (hour2 < 10 ? "0" : "") + hour2
    minutes2 = (minutes2 < 10 ? "0" : "") + minutes2;
    var currentTime2 = year2 + "-" + month2 + "-" + day2 + "T" + hour2 + ":" + minutes2 + ":00";

    $('#beginTimeChart').val(currentTime2);

    getDataByDates();

    $('#dates').click(function () {
        getDataByDates();
    });
});

function getDataByDates() {
    var beginTime = $('#beginTimeChart').val();
    var endTime = $('#endTimeChart').val();

    if (beginTime !== null && endTime !== null) {
        $.ajax({
            url: "/Home/GetDataByDates",
            data: {
                'beginTime': beginTime,
                'endTime': endTime,
                'includeBlood': true,
                'includeCarbs': true,
                'includeInsulin': true
            },
            type: "POST",
            success: function (data) {
                var a1c = Number(data);
                $('#a1c').html("A1c: " + a1c.toFixed(2));
                
                updateBSChart();
                updateInsulinChart();
                updateCarbChart();
            },
            error: function (data) {
            }
        });
    }
}
function updateChart(chData, mainTitle, yTitle, chartId) {
    console.log(chData)
    var data = [
        {
            x: chData[1],
            y: chData[2],
            type: 'scatter'
        }
    ];
    var layout = {
        title: {
            text: mainTitle,
            font: {
                //family: 'Courier New, monospace',
                size: 24
            },
            xref: 'paper',
            x: 0.05,
        },
        xaxis: {
            title: {
                text: 'Date',
                font: {
                    //family: 'Courier New, monospace',
                    size: 18,
                    color: '#7f7f7f'
                }
            },
            autorange: true,
            //range: [new Date(chData[3][0]), new Date(chData[3][1])],
            type: 'date'
        },
        yaxis: {
            title: {
                text: yTitle,
                font: {
                    //family: 'Courier New, monospace',
                    size: 18,
                    color: '#7f7f7f'
                }
            },
            autorange: true,
            range: [0, 400],
            type: 'linear'
        }
    };
    Plotly.newPlot(chartId, data, layout);
}
function updateChartInsulin(chData, mainTitle, yTitle, chartId) {
    console.log(chData)
    var dataFast = [
        {
            x: chData[1],
            y: chData[2],
            type: 'scatter'
        }
    ];
    var dataSlow = [
        {
            x: chData[3],
            y: chData[4],
            type: 'scatter'
        }
    ];
    var data = [dataFast, dataSlow];
    var layout = {
        title: {
            text: mainTitle,
            font: {
                //family: 'Courier New, monospace',
                size: 24
            },
            xref: 'paper',
            x: 0.05,
        },
        xaxis: {
            title: {
                text: 'Date',
                font: {
                    //family: 'Courier New, monospace',
                    size: 18,
                    color: '#7f7f7f'
                }
            },
            autorange: true,
            range: [new Date(chData[3][0]), new Date(chData[3][1])],
            type: 'date'
        },
        yaxis: {
            title: {
                text: yTitle,
                font: {
                    //family: 'Courier New, monospace',
                    size: 18,
                    color: '#7f7f7f'
                }
            },
            autorange: true,
            range: [0, 400],
            type: 'linear'
        }
    };
    Plotly.newPlot(chartId, data, layout);
}
function updateBSChart() {
    $.ajax({
        type: "POST",
        url: "/Home/BSChart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (chData) {
            updateChart(chData, 'Blood Sugar', 'Blood Sugar Level', 'BSChart');
        }
    });
}
function updateInsulinChart() {
    $.ajax({
        type: "POST",
        url: "/Home/InsulinChart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (chData) {
            updateChart(chData, 'Insulin', 'Units of Insulin', 'InsulinChart');
        }
    });
}
function updateCarbChart() {
    $.ajax({
        type: "POST",
        url: "/Home/CarbChart",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (chData) {
            updateChart(chData, 'Carbs', 'Carbs Eaten', 'CarbChart');
        }
    });
}