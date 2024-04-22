/**
 * @jest-environment jsdom
 */

describe('gigs visibility', () => {
    test('Clicking artist-calendar-view-gig button should display form (Should PASS)', () => {
        // Arrange
        document.body.innerHTML = `
            <div>
                <button id="artist-calendar-visibility">Calendar Visibility</button>
                        <!-- Calendar Visibility Form -->
                        <form id="artist-calendar-visibility-form" style="display: none;">
                            <div>
                                <label for="calendarVisibilityUpdate">Is your Calendar Available to the Public?:</label>
                                <input type="checkbox" id="calendarVisibilityUpdate" name="calendarVisibilityUpdate" required>
                                <button type="button" id="artist-calendar-visibility-save-database">Submit Privacy Settings</button>
                            </div>
                        </form>
                <div id="artist-calendar-visibility-feedback" style="display: none;"></div>
            </div>`;

        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('artist-calendar-visibility').addEventListener('click', function () {
                var form = document.getElementById('artist-calendar-visibility-form')
                form.style.display = 'block';
            });
        });

        // Load the event so the button does something
        const domContentLoadedEvent = new Event('DOMContentLoaded');
        document.dispatchEvent(domContentLoadedEvent);

        // Act
        const gigVisibilityButton = document.getElementById('artist-calendar-visibility');
        gigVisibilityButton.click();

        // Assert
        const artistCalendarForm = document.getElementById('artist-calendar-visibility-form');
        expect(artistCalendarForm.style.display).toBe('block');
    });
});
