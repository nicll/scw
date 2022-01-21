import {Column} from "./Column";

export class Table {
  tableId?: string;
  displayName: string;
  tableType?: string;
  ownerUserId?: string;
  columns: Array<Column>;

  constructor(displayName: string, columns: Array<Column>, owner?: string, tableId?: string, type?: string,) {
    this.tableId = tableId;
    this.displayName = displayName;
    this.tableType = type;
    this.ownerUserId = owner;
    this.columns = columns;
  }
}
