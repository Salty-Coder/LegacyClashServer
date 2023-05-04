using System;
using UCS.Core;

namespace UCS.GameFiles
{
	// Token: 0x0200001A RID: 26
	internal class ObstacleData : Data
	{
		// Token: 0x060001FB RID: 507 RVA: 0x0001062B File Offset: 0x0000E82B
		public ObstacleData(CSVRow row, DataTable dt) : base(row, dt)
		{
			base.LoadData(this, base.GetType(), row);
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060001FC RID: 508 RVA: 0x000111E1 File Offset: 0x0000F3E1
		// (set) Token: 0x060001FD RID: 509 RVA: 0x000111E9 File Offset: 0x0000F3E9
		public int AppearancePeriodHours { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060001FE RID: 510 RVA: 0x000111F2 File Offset: 0x0000F3F2
		// (set) Token: 0x060001FF RID: 511 RVA: 0x000111FA File Offset: 0x0000F3FA
		public int ClearCost { get; set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000200 RID: 512 RVA: 0x00011203 File Offset: 0x0000F403
		// (set) Token: 0x06000201 RID: 513 RVA: 0x0001120B File Offset: 0x0000F40B
		public string ClearEffect { get; set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000202 RID: 514 RVA: 0x00011214 File Offset: 0x0000F414
		// (set) Token: 0x06000203 RID: 515 RVA: 0x0001121C File Offset: 0x0000F41C
		public string ClearResource { get; set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000204 RID: 516 RVA: 0x00011225 File Offset: 0x0000F425
		// (set) Token: 0x06000205 RID: 517 RVA: 0x0001122D File Offset: 0x0000F42D
		public int ClearTimeSeconds { get; set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000206 RID: 518 RVA: 0x00011236 File Offset: 0x0000F436
		// (set) Token: 0x06000207 RID: 519 RVA: 0x0001123E File Offset: 0x0000F43E
		public string ExportName { get; set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000208 RID: 520 RVA: 0x00011247 File Offset: 0x0000F447
		// (set) Token: 0x06000209 RID: 521 RVA: 0x0001124F File Offset: 0x0000F44F
		public string ExportNameBase { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x0600020A RID: 522 RVA: 0x00011258 File Offset: 0x0000F458
		// (set) Token: 0x0600020B RID: 523 RVA: 0x00011260 File Offset: 0x0000F460
		public string ExportNameBaseNpc { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600020C RID: 524 RVA: 0x00011269 File Offset: 0x0000F469
		// (set) Token: 0x0600020D RID: 525 RVA: 0x00011271 File Offset: 0x0000F471
		public int Height { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600020E RID: 526 RVA: 0x0001127A File Offset: 0x0000F47A
		// (set) Token: 0x0600020F RID: 527 RVA: 0x00011282 File Offset: 0x0000F482
		public bool IsTombstone { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0001128B File Offset: 0x0000F48B
		// (set) Token: 0x06000211 RID: 529 RVA: 0x00011293 File Offset: 0x0000F493
		public int LootCount { get; set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0001129C File Offset: 0x0000F49C
		// (set) Token: 0x06000213 RID: 531 RVA: 0x000112A4 File Offset: 0x0000F4A4
		public int LootMultiplierForVersion2 { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000214 RID: 532 RVA: 0x000112AD File Offset: 0x0000F4AD
		// (set) Token: 0x06000215 RID: 533 RVA: 0x000112B5 File Offset: 0x0000F4B5
		public string LootResource { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000216 RID: 534 RVA: 0x000112BE File Offset: 0x0000F4BE
		// (set) Token: 0x06000217 RID: 535 RVA: 0x000112C6 File Offset: 0x0000F4C6
		public int MinRespawnTimeHours { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000218 RID: 536 RVA: 0x000112CF File Offset: 0x0000F4CF
		// (set) Token: 0x06000219 RID: 537 RVA: 0x000112D7 File Offset: 0x0000F4D7
		public bool Passable { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600021A RID: 538 RVA: 0x000112E0 File Offset: 0x0000F4E0
		// (set) Token: 0x0600021B RID: 539 RVA: 0x000112E8 File Offset: 0x0000F4E8
		public string PickUpEffect { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600021C RID: 540 RVA: 0x000112F1 File Offset: 0x0000F4F1
		// (set) Token: 0x0600021D RID: 541 RVA: 0x000112F9 File Offset: 0x0000F4F9
		public string Resource { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600021E RID: 542 RVA: 0x00011302 File Offset: 0x0000F502
		// (set) Token: 0x0600021F RID: 543 RVA: 0x0001130A File Offset: 0x0000F50A
		public int RespawnWeight { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000220 RID: 544 RVA: 0x00011313 File Offset: 0x0000F513
		// (set) Token: 0x06000221 RID: 545 RVA: 0x0001131B File Offset: 0x0000F51B
		public string SWF { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000222 RID: 546 RVA: 0x00011324 File Offset: 0x0000F524
		// (set) Token: 0x06000223 RID: 547 RVA: 0x0001132C File Offset: 0x0000F52C
		public string TID { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000224 RID: 548 RVA: 0x00011335 File Offset: 0x0000F535
		// (set) Token: 0x06000225 RID: 549 RVA: 0x0001133D File Offset: 0x0000F53D
		public int TombGroup { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000226 RID: 550 RVA: 0x00011346 File Offset: 0x0000F546
		// (set) Token: 0x06000227 RID: 551 RVA: 0x0001134E File Offset: 0x0000F54E
		public int Width { get; set; }

		// Token: 0x06000228 RID: 552 RVA: 0x00011357 File Offset: 0x0000F557
		public ResourceData GetClearingResource()
		{
			return ObjectManager.DataTables.GetResourceByName(this.ClearResource);
		}
	}
}
