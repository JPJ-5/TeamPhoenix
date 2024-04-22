/**
 * @jest-environment jsdom
 */

describe('enter calendar', () => {

    test('Clicking enter-calendar button should display artist-calendar-section (Should PASS)', () => {
        // Arrange
        document.body.innerHTML = `
        <div>
            <button id="enter-calendar">Enter Calendar</button>
            <div id="artist-calendar-section" style="display: none;">
            </div>
        </div>`;

        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('enter-calendar').addEventListener('click', function () {
                // display the artist calendar section when the button is clicked
                document.getElementById('artist-calendar-section').style.display = 'block';
            });
        });

        // Load the event so the button does something
        const domContentLoadedEvent = new Event('DOMContentLoaded');
        document.dispatchEvent(domContentLoadedEvent);

        // Act
        const enterCalendarButton = document.getElementById('enter-calendar');
        enterCalendarButton.click();

        // Assert
        const artistCalendarSection = document.getElementById('artist-calendar-section');
        expect(artistCalendarSection.style.display).toBe('block');
    });

});
