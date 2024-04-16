var tempoSlider = document.getElementById('tempoSlider');
var startStopButton = document.getElementById('startStopButton');
var mainCircle = document.getElementById('mainCircle');
var innerCircle = document.getElementById('innerCircle');
var speedDisplay = document.getElementById('speedDisplay');

/*var tickSound = new Audio('js/tick2.mp3');*/
var playing = false;
var intervalID;
var growCircle = true; // Flag to control whether to grow or shrink the inner circle

tempoSlider.addEventListener('input', function () {
    var tempo = parseInt(tempoSlider.value);
    updateTempo(tempo);
    updateSpeedDisplay(tempo); // Update speed display when slider value changes
});

startStopButton.addEventListener('click', function () {
    playing = !playing;
    startStopButton.textContent = playing ? 'Stop' : 'Start';
    if (playing) {
        var tempo = parseInt(tempoSlider.value);
        intervalID = setInterval(playTick, 60000 / tempo);
        playTick();
    } else {
        clearInterval(intervalID);
        resetInnerCircle();
    }
});

function updateTempo(tempo) {
    if (playing) {
        clearInterval(intervalID);
        intervalID = setInterval(playTick, 60000 / tempo);
    }
}

function playTick() {
    tickSound.currentTime = 0; // Reset the audio to the beginning
    tickSound.play(); // Play the tick sound

    // Grow or shrink the inner circle
    var circleSize = parseInt(innerCircle.style.width) || 0;
    var maxCircleSize = parseInt(mainCircle.style.width) || 0;
    if (growCircle) {
        circleSize += 200;
        if (circleSize >= maxCircleSize) {
            growCircle = false;
        }
    } else {
        circleSize -= 200;
        if (circleSize <= 0) {
            growCircle = true;
        }
    }
    innerCircle.style.width = circleSize + 'px';
    innerCircle.style.height = circleSize + 'px';
}

function resetInnerCircle() {
    innerCircle.style.width = '0px';
    innerCircle.style.height = '0px';
}

function updateSpeedDisplay(tempo) {
    speedDisplay.textContent = 'Speed: ' + tempo + ' BPM';
}