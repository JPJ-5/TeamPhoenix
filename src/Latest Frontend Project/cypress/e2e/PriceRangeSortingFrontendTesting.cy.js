describe('Craft Item Sorter Tests', () => {
  describe('Search Items', () => {
    it('successfully searches for items by name', () => {
      // Visit the homepage
      cy.visit('http://localhost:8800/');

      // Open the dropdown menu or modal where the CraftVerify option is located
      cy.get('#menu-btn').click();

      // Click on the 'CraftVerify' button within the dropdown
      cy.get('a[href="#craftVerify"]').click();

      // Ensure the CraftVerify view is visible
      cy.get('#craftVerifyView').should('be.visible');

      // Click on the 'Price Range Sorting' button
      cy.get('#enter-priceRangeSorting').click();

      // Now the priceRangeSortingView should be visible
      // Perform the search operation
      cy.get('#searchInput').type('405');
      cy.get('.searchbutton').click();

      // Check the results
      cy.get('#results').should('contain', '405');
    });
  });

  describe('Pagination', () => {
    it('navigates to the next page of results', () => {
      cy.visit('http://localhost:8800/');
      cy.get('#menu-btn').click();
      cy.get('a[href="#craftVerify"]').click();
      cy.get('#craftVerifyView').should('be.visible');
      cy.get('#enter-priceRangeSorting').click();
      cy.get('#nextPage').click();
      cy.get('#pageInfo').should('contain', 'Page 2');
    });
  });

  describe('Update View Format', () => {
    it('switches between grid and list views', () => {
      cy.visit('http://localhost:8800/');
      cy.get('#menu-btn').click();
      cy.get('a[href="#craftVerify"]').click();
      cy.get('#craftVerifyView').should('be.visible');
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
