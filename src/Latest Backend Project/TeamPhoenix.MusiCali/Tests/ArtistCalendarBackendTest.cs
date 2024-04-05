using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using System.Diagnostics; // for timing the length of Act
using TeamPhoenix.MusiCali.Controllers;
using TeamPhoenix.MusiCali.DataAccessLayer;
using aS = TeamPhoenix.MusiCali.Services.ArtistCalendar; // specifying the service layer as communcation between backend to backend can cause issues for controllers.
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using aC = TeamPhoenix.MusiCali.Controllers.ArtistCalendarController;
using TeamPhoenix.MusiCali.TeamPhoenix.MusiCali.DataAccessLayer.Models;
namespace TeamPhoenix.MusiCali.Tests
{
    [TestClass]
    public class ArtistCalendarBackendTest
    {
        [TestMethod]
        public void PostGig_ShouldReturnSuccessfulPost()
        {
            //Arrange
            ArtistCalendarController test = new ArtistCalendarController();
            var timer = new Stopwatch();

            //Arrange Gig Data
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            //Act
            timer.Start();
            JsonResult gigResult = test.CreateGig(gig);
            timer.Stop();

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsTrue(gigResultAssert);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
        }
        [TestMethod]
        public void PostGig_ShouldReturnUnsuccessfulPostForInvalidInput()
        {
            ArtistCalendarController test = new ArtistCalendarController();
            //Arrange Gig Data
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "P"; //should fail for being too short of a title.
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            //Act
            JsonResult gigResult = test.CreateGig(gig);

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsFalse(gigResultAssert);
        }
        [TestMethod]
        public void PostGig_ShouldReturnUnsuccessfulPostForWhitespace()
        {
            ArtistCalendarController test = new ArtistCalendarController();
            //Arrange Gig Data
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "          ";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            //Act
            JsonResult gigResult = test.CreateGig(gig);

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsFalse(gigResultAssert);
        }
        [TestMethod]
        public void PostGig_ShouldReturnUnsuccessfulPostForEmptyString()
        {
            ArtistCalendarController test = new ArtistCalendarController();
            //Arrange Gig Data
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            //Act
            JsonResult gigResult = test.CreateGig(gig);

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsFalse(gigResultAssert);
        }
        [TestMethod]
        public void PostGig_ShouldReturnUnsuccessfulPostForRepeatDateTime()
        {

            ArtistCalendarController test = new ArtistCalendarController();

            //Arrange and create Gig Data we have a matching date of
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";
            test.CreateGig(gig);


            //Act making an exact copy should create an error based on the same date.
            JsonResult gigResult = test.CreateGig(gig);

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsFalse(gigResultAssert);
        }
        [TestMethod]
        public void DeleteGig_ShouldReturnSuccessfulDelete()
        {
            ArtistCalendarController test = new ArtistCalendarController();
            //Arrange Gig Data to delete
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            test.CreateGig(gig);

            GigFindModel gigToDelete = new GigFindModel();

            gigToDelete.Username = "ArtistCalendarTest";
            gigToDelete.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);

            //Act
            //Act
            IActionResult gigResult = test.DeleteGig(gigToDelete);

            //Assert
            OkObjectResult okResponse = (gigResult as OkObjectResult)!;

            Assert.IsNotNull(okResponse); //It failed if the conversion caused it to be null.
            Assert.AreEqual(true, okResponse.Value);
        }
        [TestMethod]
        public void DeleteGig_ShouldReturnUnuccessfulDeleteForNonexistentGig()
        {
            //Arrange
            //First define the test gig we are deleting. This gig does not exist in the database
            

            ArtistCalendarController test = new ArtistCalendarController();

            GigFindModel gigToDelete = new GigFindModel();

            gigToDelete.Username = "ArtistCalendarTest";
            gigToDelete.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);

            //Act
            IActionResult gigResult = test.DeleteGig(gigToDelete);

            //Assert
            BadRequestObjectResult badRequestResponse = (gigResult as BadRequestObjectResult)!; // should be a bad request object and not an ok object
            Assert.IsNotNull(badRequestResponse);
            Assert.AreEqual("Failed to delete user gig.", badRequestResponse.Value); // message outputed for this specific bad request
        }
        [TestMethod]
        public void ViewGig_ShouldReturnSuccessfulView()
        {
            //Arrange
            ArtistCalendarController test = new ArtistCalendarController();
            var timer = new Stopwatch();

            //Arrange Gig Data to view
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            timer.Start();
            test.CreateGig(gig);
            timer.Stop();

            //define what we are testing with view.

            string username = "ArtistCalendarTest";
            string usernameOwner = "ArtistCalendarTest";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);

            //Act
            GigView? gigResult = aS.viewGig(username, usernameOwner, dateOfGig);

            //Assert
            Assert.IsNotNull(gigResult);
            Assert.AreEqual(gig.Location, gigResult.Location);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
        }
        [TestMethod]
        public void ViewGig_ShouldReturnUnsuccessfulViewForPrivateGig()
        {
            //Arrange
            ArtistCalendarController test = new ArtistCalendarController();
            //Arrange Gig Data to view
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = false;
            gig.Pay = "$5.00";

            test.CreateGig(gig);

            //define what we are testing with view.

            string username = "NotRealUserBackendTest"; //should be a nonexistent user
            string usernameOwner = "ArtistCalendarTest";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);

            //Act
            GigView? gigResult = aS.viewGig(username, usernameOwner, dateOfGig);

            //Assert
            Assert.IsNull(gigResult);
        }
        [TestMethod]
        public void ViewGig_ShouldReturnUnsuccessfulViewForNonexistentGig()
        {
            //Arrange
            ArtistCalendarController test = new ArtistCalendarController();
            GigFindModel gigToView = new GigFindModel();

            string username = "ArtistCalendarTest";
            string usernameOwner = "ArtistCalendarTest";
            DateTime dateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);


            //Act
            GigView? gigResult = aS.viewGig(username, usernameOwner, dateOfGig);

            //Assert
            Assert.IsNull(gigResult);

        }
        [TestMethod]
        public void UpdateGig_ShouldReturnSuccessfulUpdate()
        {
            //Arrange
            //string username = "ArtistCalendarTest";
            var timer = new Stopwatch();

            ArtistCalendarController test = new ArtistCalendarController();
            //Arrange Gig Data to Edit
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            test.CreateGig(gig);


            GigUpdateModel updatedGig = new GigUpdateModel();

            updatedGig.DateOfGigOriginal = new DateTime(1990, 1, 1, 9, 0, 0);
            updatedGig.Username = "ArtistCalendarTest";
            updatedGig.GigName = "EditedExampleGig"; //data we are changing
            updatedGig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            updatedGig.Location = "123 Fake St.";
            updatedGig.Description = "A cool gig that shows this is working";
            updatedGig.Visibility = true;
            updatedGig.Pay = "$5.00";

            //Act
            timer.Start();
            JsonResult gigResult = test.UpdateGig(updatedGig);
            timer.Stop();

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsTrue(gigResultAssert);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
        }
        [TestMethod]
        public void UpdateGig_ShouldReturnUnsuccessfulUpdateForIncorrectValue()
        {
            //Arrange Test User
            //string username = "ArtistCalendarTest";

            ArtistCalendarController test = new ArtistCalendarController();
            //Arrange Gig Data to Edit

            //Arrange Gig Data to delete
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            test.CreateGig(gig);

            GigUpdateModel updatedGig = new GigUpdateModel();

            updatedGig.DateOfGigOriginal = new DateTime(1990, 1, 1, 9, 0, 0);
            updatedGig.Username = "ArtistCalendarTest";
            updatedGig.GigName = "P"; //data we are changing
            updatedGig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            updatedGig.Location = "123 Fake St.";
            updatedGig.Description = "A cool gig that shows this is working";
            updatedGig.Visibility = true;
            updatedGig.Pay = "$5.00";

            //Act
            JsonResult gigResult = test.UpdateGig(updatedGig);

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsFalse(gigResultAssert);
        }
        [TestMethod]
        public void UpdateGig_ShouldReturnUnsuccessfulUpdateForWhitespace()
        {
            //Arrange Test User
            //string username = "ArtistCalendarTest";

            ArtistCalendarController test = new ArtistCalendarController();
            //Arrange Gig Data to Edit
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            test.CreateGig(gig);

            GigUpdateModel updatedGig = new GigUpdateModel();

            updatedGig.DateOfGigOriginal = new DateTime(1990, 1, 1, 9, 0, 0);
            updatedGig.Username = "ArtistCalendarTest";
            updatedGig.GigName = "            "; //data we are changing
            updatedGig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            updatedGig.Location = "123 Fake St.";
            updatedGig.Description = "A cool gig that shows this is working";
            updatedGig.Visibility = true;
            updatedGig.Pay = "$5.00";

            //Act
            JsonResult gigResult = test.UpdateGig(updatedGig);

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsFalse(gigResultAssert);
        }
        [TestMethod]
        public void UpdateGig_ShouldReturnUnsuccessfulUpdateForEmptyGigName()
        {
            //Arrange Test User
            //string username = "ArtistCalendarTest";

            ArtistCalendarController test = new ArtistCalendarController();
            //Arrange Gig Data to Edit
            //Arrange Gig Data to delete
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            test.CreateGig(gig);

            GigUpdateModel updatedGig = new GigUpdateModel();

            updatedGig.DateOfGigOriginal = new DateTime(1990, 1, 1, 9, 0, 0);
            updatedGig.Username = "ArtistCalendarTest";
            updatedGig.GigName = ""; //data we are changing
            updatedGig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            updatedGig.Location = "123 Fake St.";
            updatedGig.Description = "A cool gig that shows this is working";
            updatedGig.Visibility = true;
            updatedGig.Pay = "$5.00";

            //Act
            JsonResult gigResult = test.UpdateGig(updatedGig);

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsFalse(gigResultAssert);
        }
        [TestMethod]
        public void UpdateGigVisibility_ShouldReturnSuccessfulUpdate()
        {
            //Arrange
            ArtistCalendarController test = new ArtistCalendarController();
            var timer = new Stopwatch();

            //Arrange Gig Data to Edit
            GigCreationModel gig = new GigCreationModel();
            gig.Username = "ArtistCalendarTest";
            gig.GigName = "ExampleGig";
            gig.DateOfGig = new DateTime(1990, 1, 1, 9, 0, 0);
            gig.Location = "123 Fake St.";
            gig.Description = "A cool gig that shows this is working";
            gig.Visibility = true;
            gig.Pay = "$5.00";

            test.CreateGig(gig);

            //Arrange values needed to run UpdateGigVisibility
            GigVisibilityModel gigData = new GigVisibilityModel();
            gigData.Username = "ArtistCalendarTest";


            //What data we are editing
            gigData.GigVisibility = false;

            //Act
            timer.Start();
            JsonResult gigResult = test.UpdateGigVisibility(gigData);
            timer.Stop();

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsTrue(gigResultAssert);
            Assert.IsTrue(timer.Elapsed.TotalSeconds <= 3);
        }
        [TestMethod]
        public void UpdateGigVisibility_ShouldReturnUnsuccessfulUpdateForNonexistentGigs()
        {
            ArtistCalendarController test = new ArtistCalendarController();
            

            //Arrange values needed to run UpdateGigVisibility
            GigVisibilityModel gigData = new GigVisibilityModel();
            gigData.Username = "ArtistCalendarTest";
            //no gig is created as a user with no gigs should be able to UpdateGigVisibility.

            //What data we are editing
            gigData.GigVisibility = false;
            //Act
            JsonResult gigResult = test.UpdateGigVisibility(gigData);

            //Assert
            //First Convert json to Assertable values
            bool gigResultAssert = (bool)gigResult.Value!;
            //Assert like normal now
            Assert.IsFalse(gigResultAssert);
        }
        [TestCleanup]
        public void Cleanup()
        {
            Tester.DeleteAllRows("Gig");
        }
    }
}
