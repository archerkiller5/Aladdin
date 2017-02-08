using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicodes.WeiChat.Data.Models.WeChatStore
{
   public class TasksList: WeiChat_WeChatWithNoKeyBase
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int   ID { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        [Display(Name = "任务类型")]
        public string  TaskType_ID { get; set; }
        /// <summary>
        /// 发起人悬赏类型
        /// </summary>
        [Display(Name = "发起人悬赏类型")]
        public int  Reward_Type { get; set; }
        /// <summary>
        /// 发起人悬赏ID
        /// </summary>
        [Display(Name = "发起人悬赏ID")]
        public string  Member_ID { get; set; }
        /// <summary>
        /// 服务校区id
        /// </summary>
        [Display(Name = "服务校区")]
        public string  School_ID { get; set; }
        /// <summary>
        /// 悬赏类型
        /// </summary>
        [Display(Name = "悬赏类型")]
        public string  Reward_Name { get; set; }
        /// <summary>
        /// 悬赏值
        /// </summary>
        [Display(Name = "悬赏值")]
        public int Reward_Num { get; set; }
        /// <summary>
        /// 截止时间
        /// </summary>
        [Display(Name = "截止时间")]
        public DateTime Deadline { get; set; }
        /// <summary>
        /// 任务内容
        /// </summary>
        [Display(Name = "任务内容")]
        public string Task_Content { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int ISSort { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public EnumStuat Status { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        [Display(Name = "是否完成")]
        public Boolean IsFinish { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        [Display(Name = "是否审核")]
        public Boolean IsShenHe { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        public string Taska_Title { get; set; }

    }
    public enum EnumStuat : byte
    {
        /// <summary>
        ///     未发布 (0)
        /// </summary>
        [Display(Name = "未发布")]
        未发布 = 0,

        /// <summary>
        ///     正常 (1)
        /// </summary>
        [Display(Name = "已发布")]
        已发布 = 1,
        /// <summary>
        ///     未领取2
        /// </summary>
        [Display(Name = "未领取")]
        未领取 = 2,

        /// <summary>
        ///     以领取3
        /// </summary>
        [Display(Name = "已领取")]
        已领取 = 3,
        /// <summary>
        ///     任务中4
        /// </summary>
        [Display(Name = "任务中")]
        任务中 = 4,

        /// <summary>
        ///   待确认5
        /// </summary>
        [Display(Name = "待确认")]
        待确认 = 5,
        /// <summary>
        ///   已完成6
        /// </summary>
        [Display(Name = "已完成")]
        已完成 = 6
    }
}
