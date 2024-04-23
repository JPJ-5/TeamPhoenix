﻿var scales = {
    'A': ['A'],
    'B': ['B'],
    'C': ['C'],
    'D': ['D'],
    'E': ['E'],
    'F': ['F'],
    'G': ['G']
};

var scaleTypes = {
    'major': ['a', 'b', 'c sharp', 'd', 'e', 'f sharp', 'g sharp', 'a'],
    'minor': ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'a'],
    'Lydian': ['a', 'b', 'c sharp', 'd sharp', 'e', 'f sharp', 'g sharp', 'a'],
    'HarmonicMinor': ['a', 'b', 'c', 'd', 'e', 'f', 'g sharp', 'a'],
    'Dorian': ['a', 'b', 'c', 'd', 'e', 'f sharp', 'g', 'a'],
    'Pentatonic': ['a', 'c', 'd', 'e', 'g', 'a']
};

function generateScales() {
    var alphabet = 'ABCDEFG';
    var index = document.getElementById('note-options') ? document.getElementById('note-options').selectedIndex : 0;
    var currentNote = alphabet[index];
    var currentScales = {};
    for (var i = 0; i < alphabet.length; i++) {
        var scaleLetter = scales[i];
        currentScales[scaleLetter] = [scaleLetter];
    }
    scales = currentScales;
}

function updateDisplay() {
    var selectBox = document.getElementById('note-options');
    var selectBoxType = document.getElementById('options');

    // Check if both select boxes exist
    if (!selectBox || !selectBoxType) {
        console.error("One or more select boxes not found.");
        return;
    }

    var selectedIndex = selectBox.selectedIndex;

    // Check if selectedIndex is valid
    if (selectedIndex < 0 || selectedIndex >= selectBox.options.length) {
        console.error("Invalid selectedIndex:", selectedIndex);
        return;
    }

    // Check if a value is selected
    var selectedOption = selectBox.options[selectedIndex];
    if (!selectedOption || !selectedOption.value) {
        console.error("No option selected.");
        return;
    }

    var selectedValue = selectBox.options[selectedIndex].value;

    var selectedTypeIndex = selectBoxType.selectedIndex;

    // Check if selectedTypeIndex is valid
    if (selectedTypeIndex < 0 || selectedTypeIndex >= selectBoxType.options.length) {
        console.error("Invalid selectedTypeIndex:", selectedTypeIndex);
        return;
    }

    var selectedType = selectBoxType.options[selectedTypeIndex].value;
    var selectedScale = scales[selectedValue];
    var selectedScaleType = scaleTypes[selectedType];

    // Ensure the display element exists
    var displayElement = document.getElementById('display');
    if (!displayElement) {
        console.error("Display element not found.");
        return;
    }
    /////////////Scale Notes for A///////////////////
    if (selectedValue === 'A') {
        if (selectedType === 'major') {
            selectedScaleType = ['A', 'B', 'C#', 'D', 'E', 'F#', 'G#'];
        }
        else if (selectedType === 'minor') {
            selectedScaleType = ['A', 'B', 'C', 'D', 'E', 'F', 'G'];
        }
        else if (selectedType === 'Lydian') {
            selectedScaleType = ['A', 'B', 'C#', 'D#', 'E', 'F#', 'G#'];
        }
        else if (selectedType === 'HarmonicMinor') {
            selectedScaleType = ['A', 'B', 'C', 'D', 'E', 'F', 'G#'];
        }
        else if (selectedType === 'Dorian') {
            selectedScaleType = ['A', 'B', 'C', 'D', 'E', 'F#', 'G'];
        }
        else if (selectedType === 'Pentatonic') {
            selectedScaleType = ['A', 'C', 'D', 'E', 'G'];
        }
    }
    /////////////Scale Notes for B///////////////////
    if (selectedValue === 'B') {
        if (selectedType === 'major') {
            selectedScaleType = ['B', 'C#', 'D#', 'E', 'F#', 'G#', 'A#'];
        }
        else if (selectedType === 'minor') {
            selectedScaleType = ['B', 'C#', 'D', 'E', 'F#', 'G', 'A'];
        }
        else if (selectedType === 'Lydian') {
            selectedScaleType = ['B', 'C#', 'D#', 'E#', 'F#', 'G#', 'A#'];
        }
        else if (selectedType === 'HarmonicMinor') {
            selectedScaleType = ['B', 'C#', 'D', 'E', 'F', 'G', 'A#'];
        }
        else if (selectedType === 'Dorian') {
            selectedScaleType = ['B', 'C#', 'D', 'E', 'F#', 'G#', 'A'];
        }
        else if (selectedType === 'Pentatonic') {
            selectedScaleType = ['B', 'D', 'E', 'F#', 'A'];
        }
    }

    /////////////Scale Notes for C///////////////////
    if (selectedValue === 'C') {
        if (selectedType === 'major') {
            selectedScaleType = ['C', 'D', 'E', 'F', 'G', 'A', 'B'];
        }
        else if (selectedType === 'minor') {
            selectedScaleType = ['C', 'D', 'Eb', 'F', 'G', 'Ab', 'Bb'];
        }
        else if (selectedType === 'Lydian') {
            selectedScaleType = ['C', 'D', 'E', 'F#', 'G', 'A', 'B'];
        }
        else if (selectedType === 'HarmonicMinor') {
            selectedScaleType = ['C', 'D', 'Eb', 'F', 'G', 'Ab', 'B'];
        }
        else if (selectedType === 'Dorian') {
            selectedScaleType = ['C', 'D', 'Eb', 'F', 'G', 'A', 'Bb'];
        }
        else if (selectedType === 'Pentatonic') {
            selectedScaleType = ['C', 'Eb', 'F', 'G', 'Bb'];
        }
    }

    /////////////Scale Notes for D ///////////////////
    if (selectedValue === 'D') {
        if (selectedType === 'major') {
            selectedScaleType = ['D', 'E', 'F#', 'G', 'A', 'B', 'C#'];
        }
        else if (selectedType === 'minor') {
            selectedScaleType = ['D', 'E', 'F', 'G', 'A', 'Bb', 'C'];
        }
        else if (selectedType === 'Lydian') {
            selectedScaleType = ['D', 'E', 'F#', 'G#', 'A', 'B', 'C#'];
        }
        else if (selectedType === 'HarmonicMinor') {
            selectedScaleType = ['D', 'E', 'F', 'G', 'A', 'Bb', 'C#'];
        }
        else if (selectedType === 'Dorian') {
            selectedScaleType = ['D', 'E', 'F', 'G', 'A', 'B', 'C'];
        }
        else if (selectedType === 'Pentatonic') {
            selectedScaleType = ['D', 'F', 'G', 'A', 'C'];
        }
    }

    ////////////Scale Notes for E /////////////////////
    if (selectedValue === 'E') {
        if (selectedType === 'major') {
            selectedScaleType = ['E', 'F#', 'G#', 'A', 'B', 'C#', 'D#'];
        }
        else if (selectedType === 'minor') {
            selectedScaleType = ['E', 'F#', 'G', 'A', 'B', 'C', 'D'];
        }
        else if (selectedType === 'Lydian') {
            selectedScaleType = ['E', 'F#', 'G#', 'A#', 'B', 'C#', 'D#'];
        }
        else if (selectedType === 'HarmonicMinor') {
            selectedScaleType = ['E', 'F#', 'G', 'A', 'B', 'C', 'D#'];
        }
        else if (selectedType === 'Dorian') {
            selectedScaleType = ['E', 'F#', 'G', 'A', 'B', 'C#', 'D'];
        }
        else if (selectedType === 'Pentatonic') {
            selectedScaleType = ['E', 'G', 'A', 'B', 'D'];
        }
    }

    ////////////// Scale Notes for F /////////////////////

    if (selectedValue === 'F') {
        if (selectedType === 'major') {
            selectedScaleType = ['F', 'G', 'A', 'Bb', 'C', 'D', 'E'];
        }
        else if (selectedType === 'minor') {
            selectedScaleType = ['F', 'G', 'Ab', 'Bb', 'C', 'Db', 'Eb'];
        }
        else if (selectedType === 'Lydian') {
            selectedScaleType = ['F', 'G', 'A', 'B', 'C', 'D', 'E'];
        }
        else if (selectedType === 'HarmonicMinor') {
            selectedScaleType = ['F', 'G', 'A', 'Bb', 'C', 'D', 'E'];
        }
        else if (selectedType === 'Dorian') {
            selectedScaleType = ['F', 'G', 'Ab', 'Bb', 'C', 'D', 'Eb'];
        }
        else if (selectedType === 'Pentatonic') {
            selectedScaleType = ['F', 'Ab', 'Bb', 'C', 'Eb'];
        }
    }

    ////////////// Scale Notes for G /////////////////
    if (selectedValue === 'G') {
        if (selectedType === 'major') {
            selectedScaleType = ['G', 'A', 'B', 'C', 'D', 'E', 'F#'];
        }
        else if (selectedType === 'minor') {
            selectedScaleType = ['G', 'A', 'Bb', 'C', 'D', 'Eb', 'F'];
        }
        else if (selectedType === 'Lydian') {
            selectedScaleType = ['G', 'A', 'B', 'C#', 'D', 'E', 'F#'];
        }
        else if (selectedType === 'HarmonicMinor') {
            selectedScaleType = ['G', 'A', 'Bb', 'C', 'D', 'Eb', 'F#'];
        }
        else if (selectedType === 'Dorian') {
            selectedScaleType = ['G', 'A', 'Bb', 'C', 'D', 'E', 'F'];
        }
        else if (selectedType === 'Pentatonic') {
            selectedScaleType = ['G', 'Bb', 'C', 'D', 'F'];
        }
    }
    // Update the display content
    displayElement.textContent = selectedScale.join() + " " + selectedType + ": " + "\n" + selectedScaleType.join(', ');
}




function moveLeft() {
    var selectBox = document.getElementById('note-options');
    if (!selectBox) return;
    var selectedIndex = selectBox.selectedIndex;
    if (selectedIndex > 0) {
        selectBox.selectedIndex = selectedIndex - 1;
        updateDisplay();
    }
}

function moveRight() {
    var selectBox = document.getElementById('note-options');
    if (!selectBox) return;
    var selectedIndex = selectBox.selectedIndex;
    if (selectedIndex < selectBox.options.length - 1) {
        selectBox.selectedIndex = selectedIndex + 1;
        updateDisplay();
    }
}

function printSelectedScale() {
    updateDisplay(); // Make sure the display is up to date
    var displayContent = document.getElementById('display') ? document.getElementById('display').textContent : '';
    alert("Here Is Your Selected Scale: \n" + displayContent);
}