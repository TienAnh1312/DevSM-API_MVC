using System;
using System.Collections.Generic;

namespace PthAPI.Models​;

public partial class Login
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Remember { get; set; }
}
