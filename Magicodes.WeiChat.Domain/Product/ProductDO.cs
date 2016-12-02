using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Domain.Product
{
    public class ProductDO : DomainObjectBase
    {
        public List<ProductAllInfo> GetProductAllInfoList()
        {
            var q = (from product in Db.Product_Infos
                     select new { product, photos = Db.Site_Photos.Where(photo => product.Id == photo.GalleryId) }).ToList();
            var list = q.Select(p => new ProductAllInfo()
            {
                ProductInfo = p.product,
                Photos = p.photos.ToList()
            }).ToList();
            return list;
        }

        public ProductAllInfo GetProductInfo(Guid id)
        {
            var list = (from product in Db.Product_Infos
                        join photo in Db.Site_Photos on product.Id equals photo.GalleryId
                        join attrbute in Db.Product_ProductAttributes on product.Id equals attrbute.ProductId
                        where product.Id == id
                        select new { product, photo, attrbute }).ToList();
            if (list == null || list.Count == 0) return null;
            ProductAllInfo productInfo = new ProductAllInfo()
            {
                Photos = list.Where(p1 => p1.product.Id == id).Select(p1 => p1.photo).Distinct().ToList(),
                ProductInfo = list.First().product,
                Attributes = list.Where(p2 => p2.product.Id == id).Select(p2 => p2.attrbute).Distinct().ToList()
            };
            return productInfo;
        }
    }
}
