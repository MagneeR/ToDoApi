using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoApi.Model;
using Microsoft.EntityFrameworkCore;

namespace ToDoApi.Controllers
{
    [Route("api/[controller]")]
    public class ToDoController : Controller
    {
       
       private readonly TodoContext _context;       

       public ToDoController(TodoContext context)
        {
            _context = context;
            if (_context.Items.Count() == 0)
            {
                _context.Items.Add(new ToDoItem {
                    Name = "Comprar"
                });
                _context.Items.Add(new ToDoItem
                {
                    Name = "Estudiar"
                });
                _context.SaveChanges();//para que se guarden cosas no vale con añadirlas, hay que guardarlas con esta funcion
            }
        }

        [HttpGet]//con este atributo decimos que esto se usara para el metodo get
        public IEnumerable<ToDoItem> Get()
        {
            return _context.Items;
           
        }

        [HttpGet("{id}", Name = "GetToDoItem")] //le doy a la ruta un nombre para utilizarla mas abajo
        public async Task<IActionResult> Get([FromRoute]int id) //[FromRoute] por defecto lo sabe el metodo pero se puede poner
        {//vamos a convertir este metodo en asincrono, hay que cambiar la firma del metodo

            if (!ModelState.IsValid)//se encarga de validar los parametros que se pasan al metodo
            {
                return BadRequest(ModelState);
            }


            var item = await _context.Items
                .SingleOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
            
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ToDoItem item)
        {
            if (!ModelState.IsValid)//se encarga de validar los parametros que se pasan al metodo
            {
                return BadRequest(ModelState);
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

           //return Ok(item);
            return CreatedAtRoute("GetToDoItem", new { id = item.Id }, item);//es la forma mas estandar de hacer la api

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id) //[FromRoute] no es necesario pero queda mas claro de donde viene
        {
            if (!ModelState.IsValid)//se encarga de validar los parametros que se pasan al metodo
            {
                return BadRequest(ModelState);
            }

            var item = await _context.Items
                .SingleOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            //return NoContent();
            return Ok(item);//devulevo lo que acabo de borrar
        }

        [HttpPut ("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody]ToDoItem item)
        {
            if (!ModelState.IsValid || id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
           
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Items.Any(x => x.Id == id)) //si esta condicion se cumple se devuelve un not found
                {
                    return NotFound();
                }
                else
                {
                    throw;//si no se cumple lo de arriba volvemos a lanzar la excepcion de arriba
                }
            }
            return NoContent();
        }
    }
}