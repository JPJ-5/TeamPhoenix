const apiKey= '31b3ad64fe775eab2dd02ef8f0f5b8b46a17430519933d575d93595632ee6240';
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
    let username = 'testuser' + Date.now().toString();

    it('should create account w/ mailslurp', () => {
        cy.mailslurp({
            apiKey
        }).then(mailslurp => mailslurp.createInbox())
            .then(inbox => {
                //cy.wrap(inbox.id).as('inboxId');
                //cy.wrap(inbox.emailAddress).as('emailAddress');
                inboxId = inbox.id;
                emailAddress = inbox.emailAddress;
            }) ;
    });

    it('should register when boxes are filled out correctly', () => {
        cy.visit('http://localhost:8800/');

        //click on menu and register button
        cy.get('#menu-btn').click();
        cy.get('#show-register').click();
        cy.wait(1000)
        
        //fill in email form
        cy.get('#register-email').type(emailAddress);
        cy.get('#show-details-form').click();
        //fill in other boxes
        cy.get('#user-name').type(username);
        cy.get('#backup-email').type(emailAddress);
        //enter birthdate
        cy.get('#dob').type("2022-05-13");

        //submit
        cy.get('#finalize-registration').click();
        cy.wait(30000)
        cy.get('#register-error').should('be.empty');
    });

    it('should log in to the previously made account', () => {
        cy.visit('http://localhost:8800/');
        cy.get('#menu-btn').click();

        cy.get('#show-login').click();

        cy.get('#username').type(username);
        cy.get('#email-otp').click();
        cy.wait(15000);
        
        cy.on('window:alert', (str) => {
            expect(str).to.equal('OTP sent to your email.')
          })
        cy.on('window:confirm', () => true)

        //checks that otp was sent to disposable email
        cy.mailslurp({apiKey})
        .then(function(mailslurp) {
            return mailslurp.waitForLatestEmail(inboxId, 120000, true)
        }).then(email => /.*confirmation is: ([a-zA-Z]{8})/.exec(email.body)[1])
        .then(code => {
            cy.get('#enter-otp').type(code);
            cy.get('#submit-otp').click();
        })
        cy.wait(10000);

        cy.get('#login-error').should('be.empty');
    });

    //it('should enter correct email and OTP', () => {
    //    cy.visit('http://localhost:8800/');
    //});
});