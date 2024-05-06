//function fetchYearlyFPR() {
//    /*const username = sessionStorage.getItem("username");*/
//    const username = "thisisparthpc"
//    console.log(username);
//    const resultsDiv = document.getElementById('financialResult');
//    const loadingIndicator = document.getElementById('loading');
//    idToken = sessionStorage.getItem("idToken");
//    accessToken = sessionStorage.getItem("accessToken");
//    if (!username) {
//        alert("Please enter a username.");
//        return;
//    }
//    const frequency = "Yearly";
//    loadingIndicator.style.display = 'block';
//    resultsDiv.innerHTML = '';

//    fetch(`http://localhost:8080/SellerDashboard/api/GetFinancialReport`, {
//        method: 'GET',
//        headers: {
//            'Authentication': idToken,
//            'Authorization': accessToken,
//            'userName': username,
//            'frequency': frequency
//        }
//    })
//        .then(response => {
//            if (!response.ok) throw new Error('Failed to fetch inventory data: ' + response.statusText);
//            return response.json();
//        })
//        .then(data => {
//            if (data.length === 0) {
//                resultsDiv.innerHTML = '<p>No  Financial Report Available</p>';
//            } else {
//                const list = document.createElement('ul');
//                data.forEach(reports => {
//                    const report = document.createElement('li');
//                    report.textContent = `${reports.financialYear} (Profit: ${reports.financialProfit}) - (Revenue: ${reports.financialRevenue}) - (Sales: ${reports.sales})`;
//                    list.appendChild(report);
//                });
//                resultsDiv.appendChild(list);
//            }
//        })
//        .catch(error => {
//            console.error('Error:', error);
//            resultsDiv.innerHTML = `<p>${error.message}</p>`;
//        })
//        .finally(() => {
//            loadingIndicator.style.display = 'none';
//        });
//}
document.addEventListener('DOMContentLoaded', function () {
    const fetchFReportYear = document.getElementById('fetchYearly');
    fetchFReportYear.addEventListener('click', () => fetchFPR("Yearly"));

    const fetchFReportQuarter = document.getElementById('fetchQuarterly');
    fetchFReportQuarter.addEventListener('click', () => fetchFPR("Quarterly"));

    const fetchFReportMonth = document.getElementById('fetchMonthly');
    fetchFReportMonth.addEventListener('click', () => fetchFPR("Monthly"));
});

function fetchFPR(frequency) {
    const resultsDiv = document.getElementById('financialResult');
    const loadingIndicator = document.getElementById('loading');
    const idToken = sessionStorage.getItem("idToken");
    const accessToken = sessionStorage.getItem("accessToken");
    const username = sessionStorage.getItem("username");
    //const frequency = "Yearly";
    loadingIndicator.style.display = 'block';
    resultsDiv.innerHTML = '';

    fetch(`http://localhost:8080/SellerDashboard/api/GetFinancialReport`, {
        method: 'GET',
        headers: {
            'Authentication': idToken,
            'Authorization': accessToken,
            'userName': username,
            'frequency': frequency
        }
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to fetch financial data: ' + response.statusText);
            return response.json();
        })
        .then(data => {
            // console.log(username);
            // console.log(data);
            if (data.length === 0) {
                resultsDiv.innerHTML = '<p>No Financial Report Available</p>';
            } else {
                if (frequency === "Yearly") {
                    resultsDiv.innerHTML = `<p>${username} Yearly Financial Progress</p>`;
                    drawYearlyChart(data);
                }
                if (frequency === "Quarterly") {
                    resultsDiv.innerHTML = `<p>${username} Quarterly Financial Progress</p>`;
                    drawQuarterlyChart(data);
                }
                if (frequency === "Monthly") {
                    resultsDiv.innerHTML = `<p>${username} Monthly Financial Progress</p>`;
                    drawMonthlyChart(data)
                }
            }
        })
        .catch(error => {
            console.error('Error:', error);
            resultsDiv.innerHTML = `<p>${error.message}</p>`;
        })
        .finally(() => {
            loadingIndicator.style.display = 'none';
        });
}

function drawYearlyChart(data) {
    const canvas = document.getElementById('chartCanvas');
    const ctx = canvas.getContext('2d');

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const margin = { top: 40, right: 20, bottom: 100, left: 90 };
    const width = canvas.width - margin.left - margin.right;
    const height = canvas.height - margin.top - margin.bottom;

    const maxProfit = Math.max(...data.map(d => d.financialProfit));

    const xScale = (financialYear) => {
        const yearSpan = data[data.length - 1].financialYear - data[0].financialYear;
        return ((financialYear - data[0].financialYear) / yearSpan) * width + margin.left;
    };

    const yScale = (profit) => {
        return height - ((profit / maxProfit) * height) + margin.top;
    };

    // Setup for the filled area
    ctx.fillStyle = 'rgba(173, 216, 230, 0.5)';
    ctx.beginPath();
    ctx.moveTo(margin.left, height + margin.top);
    data.forEach((d, i) => {
        const x = xScale(d.financialYear);
        const y = yScale(d.financialProfit);
        ctx.lineTo(x, y);
    });
    ctx.lineTo(xScale(data[data.length - 1].financialYear), height + margin.top);
    ctx.closePath();
    ctx.fill();

    // Redraw the axes over the filled area
    ctx.beginPath();
    ctx.strokeStyle = '#000';
    ctx.moveTo(margin.left, margin.top);
    ctx.lineTo(margin.left, height + margin.top);
    ctx.lineTo(width + margin.left, height + margin.top);
    ctx.stroke();

    // Drawing the line and points
    ctx.strokeStyle = 'deepskyblue';
    ctx.beginPath();
    data.forEach((d, i) => {
        const x = xScale(d.financialYear);
        const y = yScale(d.financialProfit);
        ctx.fillStyle = 'deepskyblue';
        ctx.moveTo(x, y);
        ctx.arc(x, y, 5, 0, Math.PI * 2, true);
        ctx.fill();
    });
    ctx.stroke();

    // Add a label to the y-axis
    ctx.save();
    ctx.translate(20, height / 2 + margin.top);  // Changed from 10 to 20
    ctx.rotate(-Math.PI / 2);
    ctx.textAlign = 'center';
    ctx.fillStyle = '#000';
    ctx.fillText('Profit ($)', 0, 0);
    ctx.restore();

    // Adding labels to the y-axis
    const yTicks = [...new Set(data.map(d => d.financialProfit))].sort((a, b) => a - b);
    yTicks.forEach(tick => {
        const y = yScale(tick);
        ctx.fillStyle = '#000';
        ctx.textAlign = 'right';
        ctx.textBaseline = 'middle'; // Center text vertically
        ctx.fillText(`$${tick}`, margin.left - 10, y);
    });

    // Drawing x-axis labels
    data.forEach((d, i) => {
        const x = xScale(d.financialYear);
        ctx.fillStyle = '#000';
        ctx.textAlign = 'center';
        ctx.fillText(d.financialYear.toString(), x, height + margin.top + 20);
    });
}

function drawQuarterlyChart(data) {
    const canvas = document.getElementById('chartCanvas');
    const ctx = canvas.getContext('2d');

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const margin = { top: 40, right: 20, bottom: 100, left: 90 };
    const width = canvas.width - margin.left - margin.right;
    const height = canvas.height - margin.top - margin.bottom;

    const maxProfit = Math.max(...data.map(d => d.financialProfit));

    // Set to track labeled profits
    const labeledProfits = new Set();

    const xScale = (index) => {
        return margin.left + index * (width / (data.length - 1));
    };

    const yScale = (profit) => {
        return height - ((profit / maxProfit) * height) + margin.top;
    };

    // Setup for the filled area
    ctx.fillStyle = 'rgba(173, 216, 230, 0.5)';
    ctx.beginPath();
    ctx.moveTo(margin.left, height + margin.top);
    data.forEach((d, i) => {
        const x = xScale(i);
        const y = yScale(d.financialProfit);
        ctx.lineTo(x, y);
    });

    ctx.lineTo(xScale(data.length - 1), height + margin.top);
    ctx.closePath();
    ctx.fill();

    // Redraw the axes
    ctx.beginPath();
    ctx.strokeStyle = '#000';
    ctx.moveTo(margin.left, margin.top);
    ctx.lineTo(margin.left, height + margin.top);
    ctx.lineTo(width + margin.left, height + margin.top);
    ctx.stroke();

    // Drawing the line and points
    ctx.strokeStyle = 'deepskyblue';
    ctx.beginPath();
    data.forEach((d, i) => {
        const x = xScale(i);
        const y = yScale(d.financialProfit);

        ctx.fillStyle = 'deepskyblue';
        ctx.moveTo(x, y);
        ctx.arc(x, y, 5, 0, Math.PI * 2, true);
        ctx.fill();
    });
    ctx.stroke();

    // Add a label to the y-axis
    ctx.save();
    ctx.translate(20, height / 2 + margin.top);  // Changed from 10 to 20
    ctx.rotate(-Math.PI / 2);
    ctx.textAlign = 'center';
    ctx.fillStyle = '#000';
    ctx.fillText('Profit ($)', 0, 0);
    ctx.restore();

    // Adding labels to the y-axis
    ctx.fillStyle = '#000';
    ctx.textAlign = 'right';
    ctx.textBaseline = 'middle';
    data.forEach(d => {
        const y = yScale(d.financialProfit);
        if (!labeledProfits.has(d.financialProfit)) {
            ctx.fillText(`$${d.financialProfit}`, margin.left - 10, y);
            labeledProfits.add(d.financialProfit);
        }
    });

    // Redraw x-axis labels for year and quarter separately
    data.forEach((d, i) => {
        const x = xScale(i);
        ctx.fillStyle = '#000';
        ctx.textAlign = 'center';  // Align text to the center of the x position
        ctx.fillText(d.financialYear, x, height + margin.top + 20);  // Year
        ctx.fillText(`Q${d.financialQuater}`, x, height + margin.top + 40);  // Quarter below year
    });
}



function drawMonthlyChart(data) {
    const canvas = document.getElementById('chartCanvas');
    const ctx = canvas.getContext('2d');

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const margin = { top: 40, right: 20, bottom: 100, left: 90 }; // Adjust left margin for y-axis labels
    const width = canvas.width - margin.left - margin.right;
    const height = canvas.height - margin.top - margin.bottom;

    const maxProfit = Math.max(...data.map(d => d.financialProfit));
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    // Define labeledProfits Set to track which profit values have been labeled
    const labeledProfits = new Set();

    // Adjust xScale to distribute points evenly regardless of the actual date
    const xScale = (index) => {
        return margin.left + index * (width / (data.length - 1));
    };

    const yScale = (profit) => {
        return height - (profit / maxProfit) * height + margin.top;
    };

    // Draw the filled area under the line
    ctx.fillStyle = 'rgba(173, 216, 230, 0.5)';
    ctx.beginPath();
    ctx.moveTo(margin.left, height + margin.top);
    data.forEach((d, index) => {
        ctx.lineTo(xScale(index), yScale(d.financialProfit));
    });
    ctx.lineTo(xScale(data.length - 1), height + margin.top);
    ctx.closePath();
    ctx.fill();

    // Draw axes
    ctx.beginPath();
    ctx.strokeStyle = '#000';
    ctx.moveTo(margin.left, margin.top);
    ctx.lineTo(margin.left, height + margin.top);
    ctx.lineTo(width + margin.left, height + margin.top);
    ctx.stroke();

    // Draw points
    ctx.fillStyle = 'deepskyblue';
    data.forEach((d, index) => {
        const x = xScale(index);
        const y = yScale(d.financialProfit);
        ctx.beginPath();
        ctx.arc(x, y, 5, 0, 2 * Math.PI);
        ctx.fill();
    });

    // Y-axis labels for each unique profit value
    ctx.fillStyle = '#000';
    ctx.textAlign = 'right';
    ctx.textBaseline = 'middle';
    data.forEach(d => {
        const y = yScale(d.financialProfit);
        if (!labeledProfits.has(d.financialProfit)) {
            ctx.fillText(`$${d.financialProfit}`, margin.left - 10, y);
            labeledProfits.add(d.financialProfit);
        }
    });

    // X-axis labels for years and months
    ctx.textAlign = 'center';
    ctx.textBaseline = 'top';
    data.forEach((d, index) => {
        const x = xScale(index);
        ctx.fillText(`${d.financialYear}`, x, height + margin.top + 20);
        ctx.fillText(`${months[d.financialMonth - 1]}`, x, height + margin.top + 35);
    });

    // Y-axis label
    ctx.save();
    ctx.translate(20, height / 2 + margin.top);
    ctx.rotate(-Math.PI / 2);
    ctx.textAlign = 'center';
    ctx.fillText('Profit ($)', 0, 0);
    ctx.restore();
}
