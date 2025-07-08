using AutoMapper;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Modelos.Dtos;

namespace Proyecto_Api_Polleria.PolleriaMappers
{
    public class PolleriaMapper : Profile
    {
        public PolleriaMapper()
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDto>().ReverseMap();

            CreateMap<Producto, ProductoDto>().ReverseMap();
            CreateMap<Producto, CrearProductoDto>().ReverseMap();

            CreateMap<Pedido, PedidoDto>().ReverseMap();
            CreateMap<Pedido, CrearPedidoDto>().ReverseMap();

            CreateMap<DetallePedido, DetallePedidoDto>().ReverseMap();
            CreateMap<DetallePedido, CrearDetallePedidoDto>().ReverseMap();

            //Mapeo para Usuario
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioDatosDto>().ReverseMap();
            CreateMap<Usuario, UsuarioLoginDto>().ReverseMap();
            CreateMap<Usuario, UsuarioLoginRespuestaDto>().ReverseMap();
            CreateMap<Usuario, UsuarioRegistroDto>().ReverseMap();
        }
    }
}
