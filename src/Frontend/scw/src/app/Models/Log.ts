export class Log{
  constructor(id: string, timestamp: Date, userAction?: string, userId?: string, tableId? : string, tableName?: string,tableType?: string, tableAction?: string) {
    this.id = id;
    this.userId = userId;
    this.timestamp = timestamp;
    this.userAction = userAction;
    this.tableName = tableName;
    this.tableAction = tableAction;
    this.tableType = tableType;
    this.tableId = tableId;
  }
  id:string;
  userId?: string;
  timestamp: Date;
  userAction?: string;
  tableAction?: string;
  tableName?: string;
  tableType?: string;
  tableId?: string;

}
