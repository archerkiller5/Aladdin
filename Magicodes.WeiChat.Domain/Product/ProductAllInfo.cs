using Magicodes.WeiChat.Data.Models.PhotoGallery;
using Magicodes.WeiChat.Data.Models.Product;
using System.Collections.Generic;

namespace Magicodes.WeiChat.Domain.Product
{
    /// <summary>
    /// 商品相关信息
    /// </summary>
    public class ProductAllInfo
    {
        /// <summary>
        /// 商品图片
        /// </summary>
        public List<Site_Photo> Photos { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        public Product_Info ProductInfo { get; set; }

        /// <summary>
        /// 商品属性列表
        /// </summary>
        public List<Product_ProductAttribute> Attributes { get; set; }
    }
}