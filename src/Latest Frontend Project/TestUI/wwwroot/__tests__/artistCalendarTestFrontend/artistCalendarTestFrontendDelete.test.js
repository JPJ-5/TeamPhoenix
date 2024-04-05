/**
 * @jest-environment jsdom
 */

describe('delete gig', () => {
    test('Clicking artist-calendar-delete-gig button should display form (Should PASS)', () => {
        // Arrange
        document.body.innerHTML = `
            <div>
                <button id="artist-calendar-delete-gig">Delete Gig</button>
                <!-- Gig Delete Form -->
                <form id="delete-gig-form" style="display: none;">
                    <div>
                        <label for="gigDateDelete">Gig Start Time of Gig to Delete:</label>
                        <input type="datetime-local" id="gigDateDelete" name="gigDateDelete" required>
                    </div>
                    <button type="button" id="delete-gig-from-database">Delete Selected Gig</button>
                </form>
                <div id="delete-gig-feedback" style="display: none;"></div>
            </div>`;

        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('artist-calendar-delete-gig').addEventListener('click', function () {
                var form = document.getElementById('delete-gig-form');
                form.style.display = 'block';
            });
        });

        // Load the event so the button does something
        const domContentLoadedEvent = new Event('DOMContentLoaded');
        document.dispatchEvent(domContentLoadedEvent);

        // Act
        const deleteGigButton = document.getElementById('artist-calendar-delete-gig');
        deleteGigButton.click();

        // Assert
        const artistCalendarForm = document.getElementById('delete-gig-form');
        expect(artistCalendarForm.style.display).toBe('block');
    });
});
