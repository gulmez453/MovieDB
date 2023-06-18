using System.ComponentModel.DataAnnotations;

/* This code block defines 3 property named 'Name', 'Email' and 'Message'.
 The [Required] attribute indicates that the fields are required and must have a value.*/

public class ContactViewModel
{
    [Required]
    public string Name { get; set; }

     //The [EmailAddress] attribute specifies that the Email field must be a valid email address.
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Message { get; set; }
}
