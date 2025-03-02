namespace MesProject.Models
{
    /// <summary>
    /// 报警数据模型
    /// </summary>
    public class AlarmModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Num { get; set; }

        /// <summary>
        /// 报警信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 报警时间
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 报警时长, 单位:秒
        /// </summary>
        public int Duration { get; set; }

    }
}
