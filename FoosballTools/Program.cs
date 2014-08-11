namespace FoosballTools
{
    using System;
    using FoosBall.Main;
    using FoosBall.Models.Domain;

    public class Program
    {
        public static void Main(string[] args)
        {
            //var dbConnection = new Db();
            //var playersCollection = dbConnection.Dbh.GetCollection<Player>("Players");
            
            //dbConnection.Dbh.DropCollection("Users");
            
            //var usersCollection = dbConnection.Dbh.GetCollection<User>("Users");

            //foreach (var player in playersCollection.FindAll())
            //{
            //    var user = new User
            //    {
            //        Name = player.Name,
            //        Email = player.Email,
            //        Password  = player.Password,
            //        Deactivated = player.Deactivated,
            //    };

            //    Console.WriteLine(player.Name);
            //    usersCollection.Save(user);
            //}

            var recip = "j4kobbech@gmail.com";
            var subject = "Welcome to the world of tomorrow";

            var text = "Hello,\n\nThis is a test message from SendGrid.    We have sent this to you because you requested a test message be sent from your account.\n\nThis is a link to google.com: http://www.google.com\nThis is a link to apple.com: http://www.apple.com\nThis is a link to sendgrid.com: http://www.sendgrid.com\n\nThank you for reading this test message.\n\nLove,\nYour friends at SendGrid";
            var html = "<table style=\"background-color: #f5f5ff; font-family: verdana, tahoma, sans-serif; color: #000888;\"> 	<tr> 		<td id=\"template-body\"> 			<h2>Hello,</h2>  			<p> 				This is a test message from the FoosBall App. <br /><br /> 				We have sent this to you because you requested a test message be sent from your account. 			</p> 			<p><a href=\"http://www.google.com\" target=\"_blank\">This is a link to google.com</a></p> 			<p><a href=\"http://www.apple.com\" target=\"_blank\">This is a link to apple.com</a></p> 			<p><a href=\"http://www.sendgrid.com\" target=\"_blank\">This is a link to sendgrid.com</a></p> 			<p>Thank you for reading this test message.</p> 			<p> 				Regards,<br/> 				FoosBall App 			</p>  			<p> 				<img src=\"http://foosball.trustpilot.com/css/images/foosball-icon.png\" width=\"50px\" alt=\"Foosball icon\" /> 			</p> 		 		</td> 	</tr> </table>";

            Email.Send(recipient: recip, subject: subject, textBody: text, htmlBody: html, userName: user, password: pw);

        }
    }
}
