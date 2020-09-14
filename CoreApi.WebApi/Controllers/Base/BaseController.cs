using System.Threading.Tasks;
using CoreApi.Contract;
using CoreApi.Core.Service.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CoreApi.WebApi.Controllers.Base
{
    public abstract class BaseController<TService, TContract> : CommonController
        where TContract : IContract
        where TService : IServiceBase<TContract>
    {
        private readonly TService _service;
        protected BaseController(TService service)
        {
            _service = service;
        }

        [HttpOptions]
        public Task<ActionResult> Options()
        {
            return Task.FromResult<ActionResult>(Ok());
        }

        [HttpGet]
        [Route("{id}")]
        public virtual async Task<TContract> GetAsync([FromRoute]int id)
        {
            return await _service.GetAsync(id);
        }

        [HttpGet]
        public virtual async Task<ActionResult> GetAsync()
        {
            var result = await _service.GetAsync();
            return Ok(result);
        }

        [HttpPost]
        public virtual async Task<ActionResult> PostAsync([FromBody]TContract contract)
        {
            if (contract.Id == default)
            {
                var data = await _service.AddAsync(contract);
                return Ok(data);
            }
            else
                await _service.UpdateAsync(contract);

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteAsync([FromRoute]int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}