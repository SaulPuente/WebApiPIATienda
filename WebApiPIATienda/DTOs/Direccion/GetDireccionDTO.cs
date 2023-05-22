namespace WebApiPIATienda.DTOs.Direccion
{
    public class GetDireccionDTO
    {
        public int Id { get; set; }
        public string? Calle { get; set; }
        public string? Colonia { get; set; }
        public string? NumExt { get; set; }
        public string? NumInt { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public int UsuarioId { get; set; }
    }
}
