namespace Web.Dtos
{
    public class CreateUserApiRequest
    {
        public DesktopUserDto User { get; set; } = new DesktopUserDto();
        public string Password { get; set; } = "";
    }
}
