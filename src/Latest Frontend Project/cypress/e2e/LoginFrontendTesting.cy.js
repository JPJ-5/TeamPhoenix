describe('register cases', () => {
    it('should error when text fields are blank', () => {
        cy.visit('http://localhost:8800/');

        cy.get('#menu-btn').click();
        cy.get('#show-register').click();
        cy.get('#show-details-form').click();
    });
});