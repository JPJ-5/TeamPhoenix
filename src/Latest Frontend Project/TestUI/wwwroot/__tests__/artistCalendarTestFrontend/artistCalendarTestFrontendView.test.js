/**
 * @jest-environment jsdom
 */

describe('view gig', () => {
    test('Clicking artist-calendar-view-gig button should display form (Should PASS)', () => {
        // Arrange
        document.body.innerHTML = `
            <div>
                <button id="artist-calendar-view-gig">View Gig</button>

                <!-- Gig View Form -->
                <form id="view-gig-form" style="display: none;">
                    <div>
                        <label for="gigUsernameOwnerView">Username of Gig Owner:</label>
                        <input type="text" id="gigUsernameOwnerView" name="gigUsernameOwnerView" required>
                    </div>
                    <div>
                        <label for="gigDateView">Gig Start Time of Gig to View:</label>
                        <input type="datetime-local" id="gigDateView" name="gigDateView" required>
                    </div>
                    <button type="button" id="view-gig-from-database">View Selected Gig</button>
                </form>
                <div id="view-gig-feedback" style="display: none;"></div>

            </div>`;

        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('artist-calendar-view-gig').addEventListener('click', function () {
                var form = document.getElementById('view-gig-form')
                if (form.style.display === 'block') {
                    form.style.display = 'none';
                } else {
                    form.style.display = 'block';
                }
            });
        });

        // Load the event so the button does something
        const domContentLoadedEvent = new Event('DOMContentLoaded');
        document.dispatchEvent(domContentLoadedEvent);

        // Act
        const gigViewButton = document.getElementById('artist-calendar-view-gig');
        gigViewButton.click();

        // Assert
        const artistCalendarForm = document.getElementById('view-gig-form');
        expect(artistCalendarForm.style.display).toBe('block');
    });
});
