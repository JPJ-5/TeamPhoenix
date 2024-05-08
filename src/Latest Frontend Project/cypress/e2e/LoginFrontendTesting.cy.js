
describe('blank cases', () => {
    describe('register blank', () => {
        it('should error when text fields are blank', () => {
            cy.visit('http://localhost:8800/');
    
            cy.get('#menu-btn').click();
            cy.get('#show-register').click();

            cy.get('#show-details-form').click();

            cy.get('#finalize-registration').click();

            cy.get('#register-error').should('exist');
    
        });
    });
    describe('login blank fields', () => {
        it('should error when text fields are blank', () => {
            cy.visit('http://localhost:8800/');
    
            cy.get('#menu-btn').click();
            cy.get('#show-login').click();
            cy.get('#email-otp').click();
            cy.get('#login-error').should('exist');
        });
    });
});

describe('signin/signup cases', () => {
    let inboxId;
    let emailAddress;
    let username = 'cypresstester';
    beforeEach(() => {
        cy.mailslurp({
            apiKey: '7138e4e54658683686ffe75c6759a738fea535711e13bc025b5eb36c86e5c1fe'
        }).then(mailslurp => mailslurp.createInbox())
            .then(inbox => {
                //cy.wrap(inbox.id).as('inboxId');
                //cy.wrap(inbox.emailAddress).as('emailAddress');
                inboxId = inbox.id;
                emailAddress = inbox.emailAddress;
            }) ;
    });
        
    describe('registration', () => {
        it('should register when boxes are filled out correctly', () => {
            cy.visit('http://localhost:8800/');

            //click on menu and register button
            cy.get('#menu-btn').click();
            cy.get('#show-register').click();
            
            //fill in email form
            cy.get('#register-email').type(emailAddress);
            cy.get('#show-details-form').click();
            //fill in other boxes
            cy.get('#user-name').type(username);
            cy.get('#backup-email').type(emailAddress);
            //submit
            cy.get('#finalize-registration').click();
            cy.wait(6000)
            cy.get('#register-error').should('be.empty');
        });
    });

    describe('logging in successfully', () => {
        it('should enter correct email and OTP', () => {
            cy.visit('http://localhost:8800/');

            cy.get('#menu-btn').click();
            cy.get('#show-login').click();

            cy.get('#username').type(username);
            cy.get('#email-otp').click();
            cy.wait(3000);
            cy.mailslurp(function(mailslurp) {
                return mailslurp.waitForLatestEmail(inboxId, 120000, true)
            }).then(email => expect(email.body).contains('Hello'))

            cy.get('#login-error').should('exist');
        });
    });
});