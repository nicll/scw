import {Column} from "./Column";
import {Log} from "./Log";

export class Table {
  tableId?: string;
  displayName: string;
  tableType?: string;
  ownerUserId?: string;
  columns: Array<Column>;
  creationDate: Date
  logs?: Log[];

  constructor(displayName: string,  columns: Array<Column>, creationDate: Date, owner?: string, tableRef?: string, type?: string, logs?: Log[]) {
    this.tableId = tableRef;
    this.displayName = displayName;
    this.tableType = type;
    this.ownerUserId = owner;
    this.columns = columns;
    this.creationDate = creationDate;
    this.logs = logs;
  }
}
