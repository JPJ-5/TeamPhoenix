/**
 * @jest-environment jsdom
 */

describe('create gig', () => {
    test('Clicking artist-calendar-view-gig button should display form (Should PASS)', () => {
        // Arrange
        document.body.innerHTML = `
            <div>
                <button id="artist-calendar-create-gig">Create Gig</button>
                <!-- Gig Creation Form -->
                <form id="create-gig-form" style="display: none;">
                    <div>
                        <label for="gigName">Gig Name:</label>
                        <input type="text" id="gigName" name="gigName" required>
                    </div>
                    <div>
                        <label for="gigDate">Gig Start Time:</label>
                        <input type="datetime-local" id="gigDate" name="gigDate" required>
                    </div>
                    <div>
                        <label for="gigVisibility">Is this Gig Available to the Public?:</label>
                        <input type="checkbox" id="gigVisibility" name="gigVisibility" required>
                    </div>
                    <div>
                        <label for="gigDescription">Gig Description:</label>
                        <input type="text" id="gigDescription" name="gigDescription">
                    </div>
                    <div>
                        <label for="gigLocation">Gig Location:</label>
                        <input type="text" id="gigLocation" name="gigLocation">
                    </div>
                    <div>
                        <label for="gigPay">Gig Payment for Ticket:</label>
                        <input type="text" id="gigPay" name="gigPay">
                    </div>
                    <!-- Specify the type of button as buttons inside a form defaults to submit which causes the URL to change -->
                    <button type="button" id="create-gig-save-database">Submit Gig</button>
                </form>
                <div id="create-gig-feedback" style="display: none;"></div>
            </div>`;

        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('artist-calendar-create-gig').addEventListener('click', function () {
                // get the username which is needed for gig creation.
                var username = sessionStorage.getItem('username'); // Make sure this is correctly retrieving the username
                var form = document.getElementById('create-gig-form');
                form.style.display = 'block';
            });
        });

        // Load the event so the button does something
        const domContentLoadedEvent = new Event('DOMContentLoaded');
        document.dispatchEvent(domContentLoadedEvent);

        // Act
        const createGigButton = document.getElementById('artist-calendar-create-gig');
        createGigButton.click();

        // Assert
        const artistCalendarForm = document.getElementById('create-gig-form');
        expect(artistCalendarForm.style.display).toBe('block');
    });
});
