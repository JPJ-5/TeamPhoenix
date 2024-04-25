describe('Craft Item Sorter Functionality', () => {
    beforeEach(() => {
        cy.visit('http://localhost:8800/PriceRangeSorting.html'); // Adjust this if your URL is different
    });

    it('successfully loads and fetches initial items', () => {
        cy.get('#loading').should('be.visible');
        cy.get('.item-card-grid, .item-card-list').should('have.length.at.least', 1);
    });

    it('handles pagination correctly', () => {
        cy.get('#nextPage').click();
        cy.get('#pageInfo').should('contain', 'Page 2');
        cy.get('#prevPage').click();
        cy.get('#pageInfo').should('contain', 'Page 1');
    });

    it('changes view format between list and grid', () => {
        // Assuming there's a select element for changing views
        cy.get('#viewFormat').select('list');
        cy.get('.item-card-list').should('exist');
        cy.get('#viewFormat').select('grid');
        cy.get('.item-card-grid').should('exist');
    });

    it('applies price filters and checks results', () => {
        cy.get('#predefinedRanges').select('$50 to $100');
        cy.wait(1000)
        cy.get('.item-price').each(($el) => {
            const price = parseFloat($el.text().substring(1)); // Assuming price format is "$xx.xx"
            expect(price).to.be.at.least(50);
            expect(price).to.be.lessThan(100);
        });
    });

    it('validates search functionality', () => {
        cy.get('#searchInput').type('aromatic');
        cy.get('button').contains('Search').click();
        cy.wait(1000)
        cy.get('.item-name').each(($el) => {
            const name = $el.text();
            expect(name.toLowerCase()).to.include('aromatic');
        });
    });

    it('ensures that changing the page size updates the number of items displayed', () => {
        cy.get('#pageSize').select('10');
        cy.wait(1000)
        cy.get('.item-card-grid, .item-card-list').should('have.length', 10);
    });

});