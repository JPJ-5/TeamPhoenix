var scales = {
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
        var scaleLetter = alphabet[i];
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

    var selectedValue = selectedOption.value;

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


