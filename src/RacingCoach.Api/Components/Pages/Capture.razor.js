let chart = null;

export function initializeChart() {
    const ctx = document.getElementById('telemetryChart');
    if (!ctx) return;
    
    // Destruir gráfico existente si hay uno
    if (chart) {
        chart.destroy();
        chart = null;
    }
    
    chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: [],
            datasets: [
                {
                    label: 'Speed (km/h)',
                    data: [],
                    borderColor: 'rgb(59, 130, 246)',
                    backgroundColor: 'rgba(59, 130, 246, 0.1)',
                    yAxisID: 'y',
                    tension: 0.4,
                    borderWidth: 2
                },
                {
                    label: 'Throttle (%)',
                    data: [],
                    borderColor: 'rgb(34, 197, 94)',
                    backgroundColor: 'rgba(34, 197, 94, 0.1)',
                    yAxisID: 'y1',
                    tension: 0.4,
                    borderWidth: 2
                },
                {
                    label: 'Brake (%)',
                    data: [],
                    borderColor: 'rgb(239, 68, 68)',
                    backgroundColor: 'rgba(239, 68, 68, 0.1)',
                    yAxisID: 'y1',
                    tension: 0.4,
                    borderWidth: 2
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                duration: 0
            },
            interaction: {
                mode: 'index',
                intersect: false
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Time'
                    },
                    ticks: {
                        maxTicksLimit: 10
                    }
                },
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    title: {
                        display: true,
                        text: 'Speed (km/h)'
                    },
                    min: 0,
                    max: 300
                },
                y1: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    title: {
                        display: true,
                        text: 'Throttle / Brake (%)'
                    },
                    min: 0,
                    max: 100,
                    grid: {
                        drawOnChartArea: false
                    }
                }
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'top'
                }
            }
        }
    });
}

export function updateChartData(labels, speedData, throttleData, brakeData) {
    if (chart) {
        chart.data.labels = labels;
        chart.data.datasets[0].data = speedData;
        chart.data.datasets[1].data = throttleData;
        chart.data.datasets[2].data = brakeData;
        chart.update('none');
    }
}

export function destroyChart() {
    if (chart) {
        chart.destroy();
        chart = null;
    }
}
