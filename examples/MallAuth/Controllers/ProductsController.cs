using DbWrappedApp.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MallAuth.Controllers
{
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        [Route("tmpl/{subCategoryId:int}")] 
        public IEnumerable<object> GetAll(int subCategoryId)
        {
            return _.service.getProductList(subCategoryId);
            //return result;
        }


        [Route("types")]
        public IEnumerable<object> GetTypes()
        {
             return _.service.getSubCategory();
        }
        // GET api/products/5
        [Route("{prodId:int}")]
        public object Get(int prodId)
        {
            return _.service.getProductInfos(prodId);
        }

        // POST api/products
        public void Post([FromBody]string value)
        {
        }

        // PUT api/products/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/products/5
        public void Delete(int id)
        {
        }
    }
}
