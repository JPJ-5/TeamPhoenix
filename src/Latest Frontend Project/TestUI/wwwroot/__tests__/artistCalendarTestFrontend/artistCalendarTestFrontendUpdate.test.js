/**
 * @jest-environment jsdom
 */

describe('delete gig', () => {
    test('Clicking artist-calendar-delete-gig button should display form (Should PASS)', () => {
        // Arrange
        document.body.innerHTML = `
            <div>
                <button id="artist-calendar-update-gig">Update Gig</button>
                <!-- Gig Update Form -->
                <form id="update-gig-form" style="display: none;">
                    <div>
                        <label for="gigDateOriginal">Original Gig Start Time:</label>
                        <input type="datetime-local" id="gigDateOriginal" name="gigDateOriginal" required>
                    </div>
                    <div>
                        <label for="gigNameUpdate">New Gig Name:</label>
                        <input type="text" id="gigNameUpdate" name="gigNameUpdate" required>
                    </div>
                    <div>
                        <label for="gigDateUpdate">New Gig Start Time:</label>
                        <input type="datetime-local" id="gigDateUpdate" name="gigDateUpdate" required>
                    </div>
                    <div>
                        <label for="gigVisibilityUpdate">Is this Gig Available to the Public?:</label>
                        <input type="checkbox" id="gigVisibilityUpdate" name="gigVisibilityUpdate" required>
                    </div>
                    <div>
                        <label for="gigDescriptionUpdate">New Gig Description:</label>
                        <input type="text" id="gigDescriptionUpdate" name="gigDescriptionUpdate">
                    </div>
                    <div>
                        <label for="gigLocationUpdate">New Gig Location:</label>
                        <input type="text" id="gigLocationUpdate" name="gigLocationUpdate">
                    </div>
                    <div>
                        <label for="gigPayUpdate">New Gig Payment for Ticket:</label>
                        <input type="text" id="gigPayUpdate" name="gigPayUpdate">
                    </div>
                    <!-- Specify the type of button as buttons inside a form defaults to submit which causes the URL to change -->
                    <button type="button" id="update-gig-save-database">Submit Updated Gig</button>
                </form>
                <div id="update-gig-feedback" style="display: none;"></div>
            </div>`;

        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('artist-calendar-update-gig').addEventListener('click', function () {
                var form = document.getElementById('update-gig-form');
                form.style.display = 'block';
            });
        });

        // Load the event so the button does something
        const domContentLoadedEvent = new Event('DOMContentLoaded');
        document.dispatchEvent(domContentLoadedEvent);

        // Act
        const updateGigButton = document.getElementById('artist-calendar-update-gig');
        updateGigButton.click();

        // Assert
        const artistCalendarForm = document.getElementById('update-gig-form');
        expect(artistCalendarForm.style.display).toBe('block');
    });
});
