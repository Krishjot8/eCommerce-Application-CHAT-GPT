﻿using AutoMapper;
using eCommerce_App.Data;
using eCommerce_App.DTOs;
using eCommerce_App.ExceptionMiddleware;
using eCommerce_App.Models;
using eCommerce_App.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce_App.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository productRepository, IMapper mapper, ILogger<ProductsController> logger)
        {

            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }






        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {



            var products = await _productRepository.GetProductsAsync();
            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return Ok(productDtos);
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {

            try

            {

                var product = await _productRepository.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound(new { Message = "The product or ID you are searching for does not exist" });
                }

                var productDto = _mapper.Map<ProductDto>(product);
                return Ok(productDto);

            }

            catch (Exception ex)
            {

                _logger.LogError(ex, "The product or ID you are searching for does not exist");




                return StatusCode(404, "The product or ID you are searching for does not exist");




            }


        }




        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(ProductDto productDto)
        {

            try

            {
                var product = _mapper.Map<Product>(productDto);

                await _productRepository.AddProductAsync(product);

                var createdProductDto = _mapper.Map<ProductDto>(product);

                //Return the created productDto or another appropriate response
                return CreatedAtAction(nameof(GetProduct), new { id = createdProductDto.Id }, createdProductDto);

            }

            catch (Exception)

            {


                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse { Message = "There was an Internal Server Error" });


            }
        }





        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductDto updatedProductDto)
        {
            if (id != updatedProductDto.Id)
            {
                return BadRequest();
            }

            var existingProduct = await _productRepository.GetProductByIdAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            _mapper.Map(updatedProductDto, existingProduct);
            try
            {

                await _productRepository.UpdateProductAsync(existingProduct);

                await _productRepository.SaveChangesAsync();

            }

            catch (DbUpdateConcurrencyException)
            {
                if (await _productRepository.ProductExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {

                    throw;
                }
            }

            return NoContent();


        }


        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteProduct(int id)
        {
            try {

                var product = await _productRepository.GetProductByIdAsync(id);

                if (product == null) 
                {

                    return NotFound(new { message = "Not Found" });
                }

                await _productRepository.DeleteProductAsync(id);

                _logger.LogInformation($"Product with ID {id} deleted.");

                return NoContent();
            }
            catch (Exception ex)
           
            {
                _logger.LogError(ex,"Error deleting the product");
                return StatusCode(500, "An error occured while processing your request");
            }

        
        
        
        }
           


            



        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<ProductBrandDto>>> GetProductBrands()
        {



            var productbrands = await _productRepository.GetProductBrandsAsync();
            var productBrandDtos = _mapper.Map<List<ProductBrandDto>>(productbrands);
            return Ok(productBrandDtos);
        }



        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<ProductTypeDto>>> GetProductTypes()
        {



            var producttypes = await _productRepository.GetProductTypesAsync();
            var productTypeDtos = _mapper.Map<List<ProductTypeDto>>(producttypes);
            return Ok(productTypeDtos);

        }



    }
}


