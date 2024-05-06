describe('register cases', () => {
    describe('register blank', () => {
        it('should error when text fields are blank', () => {
            cy.visit('http://localhost:8800/');
    
            cy.get('#menu-btn').click();
            cy.get('#show-register').click();

            cy.get('#show-details-form').click();

            cy.get('#register-details-form').click();

            cy.get('#register-error').should('exist');
    
        });
    });
    
    describe('registration', () => {
        it('should register when boxes are filled out correctly', () => {
            cy.visit('http://localhost:8800/');

            cy.get('#menu-btn').click();
            cy.get('#show-register').click();
            let testUsername = (Math.random() + 1).toString(36).substring(7);
            console.log(testUsername);
            //
            cy.get('#show-details-form').click();
            //
            cy.get('#register-details-form').click();
        });
    });
});

describe('login cases', () => {
    it('should error when text fields are blank', () => {
        cy.visit('http://localhost:8800/');

        cy.get('#menu-btn').click();
        cy.get('#show-login').click();
        cy.get('#email-otp').click();
        cy.get('#login-error').should('exist');
    });
});