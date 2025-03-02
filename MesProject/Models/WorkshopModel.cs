namespace MesProject.Models
{
    /// <summary>
    /// 车间
    /// </summary>
    public class WorkshopModel
    {
        /// <summary>
        /// 车间名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 作业数量
        /// </summary>
        public int WorkingNum { get; set; }

        /// <summary>
        /// 等待数量
        /// </summary>
        public int WaitingNum { get; set; }

        /// <summary>
        /// 故障数量
        /// </summary>
        public int BreakdownNum { get; set; }

        /// <summary>
        /// 停机数量
        /// </summary>
        public int HaltNum { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalNum => WorkingNum + WaitingNum + BreakdownNum + HaltNum;
    }
}
