describe('Craft Item Sorter Tests', () => {
  describe('Search Items', () => {
      it('successfully searches for items by name', () => {
          // Visit the homepage
          cy.visit('http://localhost:8800/');

          // Open the dropdown menu or modal where the Price Range Sorting option is located
          // Assuming the hamburger icon needs to be clicked to show the dropdown
          cy.get('#menu-btn').click();

          // Click on the 'Price Range Sorting' button within the dropdown
          cy.get('#enter-priceRangeSorting').click();

          // Now the priceRangeSortingView should be visible
          // Perform the search operation
          cy.get('#searchInput').type('string');
          cy.get('.searchbutton').click();

          // Check the results
          cy.get('#results').should('contain', 'string');
      });
  });
  
  describe('Filter Items by Price', () => {
    it('filters items within a specified price range', () => {
      cy.visit('http://localhost:8800/'); // Visit the main page instead of a specific HTML file

      // Assuming you need to open the dropdown and click the Price Range Sorting button
      cy.get('#menu-btn').click(); // Open the dropdown or menu
      cy.get('#enter-priceRangeSorting').click(); // Click the Price Range Sorting button

      // Mock the API response
      cy.intercept('GET', 'http://localhost:8080/Item/api/pagedFilteredItems?pageNumber=1&pageSize=7&bottomPrice=50&topPrice=100', {
        statusCode: 200,
        body: {
          items: [
            { name: 'Craft Item 1', price: 55.00 },
            { name: 'Craft Item 2', price: 75.00 },
            { name: 'Craft Item 3', price: 99.99 }
          ],
          totalCount: 3
        }
      }).as('getFilteredItems');

      // Trigger the filtering
      cy.get('#predefinedRanges').select('$50 to $100');
      cy.wait('@getFilteredItems');

      // Check if the items are within the expected price range
      cy.get('.item-price').each(($el) => {
        const price = parseFloat($el.text().replace('$', ''));
        expect(price).to.be.at.least(50);
        expect(price).to.be.below(100);
      });
    });
  });

  describe('Pagination', () => {
    it('navigates to the next page of results', () => {
      cy.visit('http://localhost:8800/');
      cy.get('#menu-btn').click();
      cy.get('#enter-priceRangeSorting').click();
      cy.get('#nextPage').click();
      cy.get('#pageInfo').should('contain', 'Page 2');
    });
  });

  describe('Update View Format', () => {
    it('switches between grid and list views', () => {
        cy.visit('http://localhost:8800/');
        cy.get('#menu-btn').click();
        cy.get('#enter-priceRangeSorting').click();

        // Select 'List View' and check if the class is correctly applied
        cy.get('#viewFormat').select('List View');
        cy.get('#results').should('have.class', 'results item-card-list'); // Ensure the class change takes effect

        // Select 'Grid View' and check if the class is correctly applied
        cy.get('#viewFormat').select('Grid View');
        cy.get('#results').should('have.class', 'results item-card-grid'); // Ensure the class change takes effect
    });
  });
});