using DebtService.Repository;

namespace DebtService.Services
{
    public class Service : IService<OrderSend, OrderReceive>
    {

        private readonly IRepository<Order> _repository;
        private readonly Client<ProductSend> _client;

        public Service(IRepository<Order> repository)
        {
            _repository = repository;
            _client = new Client<ProductSend>("http://localhost:5066/api/Product/");
        }

        public Task<OrderSend> Create(OrderReceive receive)
        {
            Order order = DtoToEntity(receive);
            _repository.Create(order);
            return EntityToDto(order);

            //return EntityToDto(_repository.Create(DtoToEntity(receive)));
        }

        public async Task<List<OrderSend>> GetAll()
        {
            List<Order> orders = _repository.GetAll();
            List<OrderSend> orderSends = new List<OrderSend>();
            foreach (Order send in orders)
            {
                orderSends.Add(await EntityToDto(send));
            }
            return orderSends;
        }

        public async Task<OrderSend> GetById(int id)
        {
            return await EntityToDto(_repository.GetById(id));
        }


        private Order DtoToEntity(OrderReceive receive)
        {
            return new Order() { CommandeNumber = receive.commandeNumber, ProductIds = receive.productIds };
        }

        private async Task<OrderSend> EntityToDto(Order order)
        {
            OrderSend send = new OrderSend() { Id = order.Id, CommandeNumber = order.CommandeNumber };


            foreach (var productId in order.ProductIds)
            {
                send.Product.Add(await _client.GetRequest(productId.ToString()));
            }

            Console.WriteLine(send);

            return send;
        }
    }
