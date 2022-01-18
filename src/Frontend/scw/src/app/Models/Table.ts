import {Column} from "./Column";

export class Table {
  tableId?: string;
  displayName: string;
  tableType?: string;
  ownerUserId?: string;
  columns: Array<Column>;

  constructor(displayName: string, columns: Array<Column>, owner?: string, tableRef?: string, type?: string,) {
    this.tableId = tableRef;
    this.displayName = displayName;
    this.tableType = type;
    this.ownerUserId = owner;
    this.columns = columns;
  }
}
