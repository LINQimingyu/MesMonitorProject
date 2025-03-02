namespace MesProject.Models
{
    public class MachineModel
    {
        /// <summary>
        /// 机台名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 机台状态
        /// </summary>
        public string Status { get; set; }

        public int PlanNum { get; set; }

        public int CompleteNum { get; set; }

        /// <summary>
        /// 工单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 完成率
        /// </summary>
        public int CompleteRate => (int)(CompleteNum * 100.0 / PlanNum);
    }
}
