using Magicodes.WeiChat.Data.Models.PhotoGallery;
using Magicodes.WeiChat.Infrastructure;
using System;
using System.IO;

namespace Magicodes.WeiChat.Domain.PhotoGallery
{
    /// <summary>
    /// 图片领域模型
    /// </summary>
    public class PhotoDO : DomainObjectBase
    {
        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="photoGalleryId">图库Id</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="stream">文件流</param>
        /// <returns></returns>
        public Guid Add(Guid photoGalleryId, string fileName, Stream stream)
        {
            //TODO:文件验证
            var photo = new Site_Photo()
            {
                FileName = fileName,
                GalleryId = photoGalleryId
            };
            //获取存储提供程序
            var storageProvider = SiteStorageManager.Current.PhotoGalleryStorageProvider;
            var dirName = photoGalleryId.ToString("N");
            var trueName = photo.Id.ToString("N") + Path.GetExtension(fileName);
            //上传文件
            storageProvider.SaveBlobStream(dirName, trueName, stream);
            //设置路径
            photo.Url = storageProvider.GetBlobUrl(dirName, trueName);
            SetModel(photo, default(Guid));
            Db.Site_Photos.Add(photo);
            Db.SaveChanges();
            return photo.Id;
        }
        /// <summary>
        /// 伪删除
        /// </summary>
        /// <param name="photoId"></param>
        public void Remove(Guid photoId)
        {
            var model = Db.Site_Photos.Find(photoId);
            model.IsDeleted = true;
            SetModel(model, photoId);
            Db.SaveChanges();
        }
    }
}
