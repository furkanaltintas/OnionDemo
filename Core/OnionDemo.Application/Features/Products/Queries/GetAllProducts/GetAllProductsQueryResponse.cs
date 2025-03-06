﻿namespace OnionDemo.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryResponse
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
}