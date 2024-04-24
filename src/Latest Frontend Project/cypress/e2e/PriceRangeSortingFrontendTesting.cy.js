describe('Initial Page Load', () => {
  it('successfully loads and fetches initial items', () => {
      cy.visit('http://localhost:8800/PriceRangeSorting.html') // Adjust this if your base URL differs
      cy.get('#loading').should('be.visible');
  });
});

describe('Initial Page Load', () => {
    it('successfully loads and fetches initial items', () => {
        cy.visit('http://localhost:8800/PriceRangeSorting.html') // Adjust this to your application's actual URL
        cy.get('#loading').should('be.visible');
        cy.get('#results').should('not.contain', 'No items found.');
        cy.get('.item-card').should('have.length.at.least', 1); // Assumes there are items to display
    });
});

describe('Search Functionality', () => {
    it('accepts input and fetches search results', () => {
        cy.visit('http://localhost:8800/PriceRangeSorting.html')
        cy.get('#searchInput').type('Aromatic Candle Set{enter}');
        cy.get('#loading').should('be.visible');
    });

    it('validates search input and shows alert on invalid input', () => {
        cy.visit('http://localhost:8800/PriceRangeSorting.html')
        cy.get('#searchInput').type('1234{enter}');
        cy.on('window:alert', (str) => {
            expect(str).to.equal(`Please enter a valid search term using only letters.`);
        });
    });
});

describe('Pagination Controls', () => {
    it('navigates to the last page and checks if the next button is disabled', () => {
        cy.visit('http://localhost:8800/PriceRangeSorting.html');
        cy.intercept('GET', '**/pagedFilteredItems*').as('fetchItems');

        // Function to recursively click 'Next' until it's disabled
        function navigateToLastPage() {
            cy.get('#nextPage').then($btn => {
                if (!$btn.is(':disabled')) {
                    cy.wrap($btn).click();
                    // Wait for the AJAX call to complete after each click
                    cy.wait('@fetchItems', { timeout: 3000 }); // Increased timeout for safety
                    navigateToLastPage(); // Recursively click next until disabled
                } else {
                    // Once disabled, we perform the assertion to ensure it's actually disabled
                    cy.wrap($btn).should('be.disabled');
                }
            });
        }

        // Start navigating through the pages
        navigateToLastPage();
    });
});



describe('Price Range Filter', () => {
    it('filters items by price range after setting page size to 5', () => {
        cy.visit('http://localhost:8800/PriceRangeSorting.html');

        // Intercepting AJAX call
        cy.intercept('GET', '/Item/api/pagedFilteredItems*').as('getFilteredItems');

        cy.wait(2000); // Wait for 2000 milliseconds
        
        // Set the page size to 20
        cy.get('#pageSize').select('5');

        // Select price range $50 to $100
        cy.get('#predefinedRanges').select('$50 to $100');

        // Wait for the AJAX request to complete
        cy.wait('@getFilteredItems');

        // Ensure the loading completes and the items are displayed
        cy.get('.item-price', { timeout: 10000 }).should('be.visible').each(($price) => {
            const priceText = $price.text().trim(); // Gets the text and trims any whitespace
            const price = parseInt(priceText.substring(1)); // Removes the dollar sign and parses the number

            // Asserting each item's price is within the specified range
            expect(price).to.be.at.least(50);
            expect(price).to.be.lessThan(101); // Including $100
        });
    });
});

