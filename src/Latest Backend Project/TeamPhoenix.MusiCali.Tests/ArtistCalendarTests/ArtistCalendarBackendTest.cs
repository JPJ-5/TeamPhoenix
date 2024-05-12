using System.Diagnostics; // for timing the length of Act.
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Services;
using Microsoft.Extensions.Configuration;
namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class ArtistCalendarBackendTest
    {
        private IConfiguration configuration;

        public ArtistCalendarBackendTest()
        {
            // Load configuration from appsettings.json in the test project
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
        [TestMethod]
        public void PostGig_ShouldReturnSuccessfulPost()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);
            var timer = new Stopwatch();

            //Arrange Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";

            //Act
            timer.Start();
            Result testResult = test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);
            timer.Stop();

            //Assert
            //Assert like normal now
            Assert.IsTrue(testResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);

            //Clean up
            test.DeleteGigService(username, dateOfGig);
        }
        [TestMethod]
        public void PostGig_ShouldReturnUnsuccessfulPostForInvalidInput()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);

            //Arrange Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "P"; // should fail because length of gig name is short
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";

            //Act
            Result testResult = test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Assert
            Assert.IsFalse(testResult.Success);
        }
        [TestMethod]
        public void PostGig_ShouldReturnUnsuccessfulPostForWhitespace()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);

            //Arrange Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "             "; // should fail because gig name is only whitespace.
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";

            //Act
            Result testResult = test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Assert
            Assert.IsFalse(testResult.Success);
        }
        [TestMethod]
        public void PostGig_ShouldReturnUnsuccessfulPostForEmptyString()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);

            //Arrange Gig Data
            string username = "ArtistCalendarTest";
            string gigName = ""; // should fail because length of gig name is short
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";

            //Act
            Result testResult = test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Assert
            Assert.IsFalse(testResult.Success);
        }
        [TestMethod]
        public void PostGig_ShouldReturnUnsuccessfulPostForRepeatDateTime()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);

            //Arrange Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";
            //arrange data that is being copied.
            test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Act
            Result testResult = test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Assert
            //Assert like normal now
            Assert.IsFalse(testResult.Success);

            //Clean Up
            test.DeleteGigService(username, dateOfGig);
        }
        [TestMethod]
        public void DeleteGig_ShouldReturnSuccessfulDelete()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);
            var timer = new Stopwatch();

            //Arrange Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";
            test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Act
            timer.Start();
            Result testResult = test.DeleteGigService(username, dateOfGig);
            timer.Stop();

            //Assert
            //Assert like normal now
            Assert.IsTrue(testResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);

            //Clean Up
            test.DeleteGigService(username, dateOfGig);
        }
        [TestMethod]
        public void DeleteGig_ShouldReturnUnuccessfulDeleteForNonexistentGig()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);

            //Arrange Gig Data
            string username = "ArtistCalendarTest";
            DateTime dateOfGig = new DateTime(1700, 1, 1, 9, 0, 0);

            //Act
            Result testResult = test.DeleteGigService(username, dateOfGig);

            //Assert
            //Assert like normal now
            Assert.IsFalse(testResult.Success);
        }
        [TestMethod]
        public void ViewGig_ShouldReturnSuccessfulView()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);
            var timer = new Stopwatch();

            //Arrange Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";
            test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Act
            timer.Start();
            GigView? testResult = test.ViewGigService(username, username, dateOfGig);
            timer.Stop();

            //Assert
            Assert.IsNotNull(testResult);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);

            //Clean Up
            test.DeleteGigService(username, dateOfGig);
        }
        [TestMethod]
        public void ViewGig_ShouldReturnUnsuccessfulViewForPrivateGig()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);

            //Arrange Gig Data to view
            string usernameOwner = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = false;
            string pay = "$5.00";
            test.CreateGigService(usernameOwner, gigName, dateOfGig, visibility, description, location, pay);

            //define what we are testing with view.

            string usernameViewingGig = "NotRealUserBackendTest"; //should be a nonexistent user

            //Act
            GigView? gigResult = test.ViewGigService(usernameViewingGig, usernameOwner, dateOfGig);

            //Assert
            Assert.IsNull(gigResult);
            //Clean up
            test.DeleteGigService(usernameOwner, dateOfGig);
        }
        [TestMethod]
        public void ViewGig_ShouldReturnUnsuccessfulViewForNonexistentGig()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);

            string username = "ArtistCalendarTest";
            string usernameOwner = "ArtistCalendarTest";
            DateTime dateOfGig = new DateTime(1700, 1, 1, 9, 0, 0);


            //Act
            GigView? gigResult = test.ViewGigService(username, usernameOwner, dateOfGig);

            //Assert
            Assert.IsNull(gigResult);
        }
        [TestMethod]
        public void UpdateGig_ShouldReturnSuccessfulUpdate()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);
            var timer = new Stopwatch();

            //Arrange and create original Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";
            test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Arrange data beingg changed
            string updatedGigName = "UpdatedExampleGig";

            //Act
            timer.Start();
            Result testResult = test.UpdateGigService(dateOfGig, username, updatedGigName, dateOfGig, visibility, description, location, pay);
            timer.Stop();

            //Assert
            //Assert like normal now
            Assert.IsTrue(testResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);

            //Clean up
            test.DeleteGigService(username, dateOfGig);
        }
        [TestMethod]
        public void UpdateGig_ShouldReturnUnsuccessfulUpdateForIncorrectValue()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);
            var timer = new Stopwatch();

            //Arrange and create original Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";
            test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Arrange data beingg changed
            string updatedGigName = "P"; // incorrect value so it shouldn't update the gig

            //Act
            timer.Start();
            Result testResult = test.UpdateGigService(dateOfGig, username, updatedGigName, dateOfGig, visibility, description, location, pay);
            timer.Stop();

            //Assert
            //Assert like normal now
            Assert.IsFalse(testResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);

            //Clean up
            test.DeleteGigService(username, dateOfGig);
        }
        [TestMethod]
        public void UpdateGig_ShouldReturnUnsuccessfulUpdateForWhitespace()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);
            var timer = new Stopwatch();

            //Arrange and create original Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";
            test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Arrange data beingg changed
            string updatedGigName = "              "; // incorrect value so it shouldn't update the gig

            //Act
            timer.Start();
            Result testResult = test.UpdateGigService(dateOfGig, username, updatedGigName, dateOfGig, visibility, description, location, pay);
            timer.Stop();

            //Assert
            //Assert like normal now
            Assert.IsFalse(testResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);

            //Clean up
            test.DeleteGigService(username, dateOfGig);
        }
        [TestMethod]
        public void UpdateGig_ShouldReturnUnsuccessfulUpdateForEmptyGigName()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);
            var timer = new Stopwatch();

            //Arrange and create original Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";
            test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Arrange data beingg changed
            string updatedGigName = ""; // incorrect value so it shouldn't update the gig

            //Act
            timer.Start();
            Result testResult = test.UpdateGigService(dateOfGig, username, updatedGigName, dateOfGig, visibility, description, location, pay);
            timer.Stop();

            //Assert
            //Assert like normal now
            Assert.IsFalse(testResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);

            //Clean up
            test.DeleteGigService(username, dateOfGig);
        }
        [TestMethod]
        public void UpdateGigVisibility_ShouldReturnSuccessfulUpdate()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);
            var timer = new Stopwatch();

            //Arrange and create original Gig Data
            string username = "ArtistCalendarTest";
            string gigName = "ExampleGig";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            string location = "123 Fake St.";
            string description = "A cool gig that shows this is working";
            bool visibility = true;
            string pay = "$5.00";
            test.CreateGigService(username, gigName, dateOfGig, visibility, description, location, pay);

            //Arrange data beingg changed
            bool updatedVisibility = false; // incorrect value so it shouldn't update the gig

            //Act
            timer.Start();
            Result testResult = test.UpdateGigVisibilityService(username, updatedVisibility);
            timer.Stop();

            //Assert
            //Assert like normal now
            Assert.IsTrue(testResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);

            //Clean up
            test.DeleteGigService(username, dateOfGig);
        }
        [TestMethod]
        public void UpdateGigVisibility_ShouldReturnUnsuccessfulUpdateForNonexistentGigs()
        {
            //Arrange
            ArtistCalendarService test = new ArtistCalendarService(configuration);
            var timer = new Stopwatch();

            string username = "ArtistCalendarTest";
            bool updatedVisibility = false;

            //Act
            timer.Start();
            Result testResult = test.UpdateGigVisibilityService(username, updatedVisibility);
            timer.Stop();

            //Assert
            //Assert like normal now
            Assert.IsFalse(testResult.Success);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
        }
    }
}
