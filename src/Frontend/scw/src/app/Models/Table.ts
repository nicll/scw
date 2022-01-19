import {Column} from "./Column";

export class Table {
  tableId?: string;
  displayName: string;
  tableType?: string;
  ownerUserId?: string;
  columns: Array<Column>;
  creationDate: Date

  constructor(displayName: string,  columns: Array<Column>, creationDate: Date, owner?: string, tableRef?: string, type?: string) {
    this.tableId = tableRef;
    this.displayName = displayName;
    this.tableType = type;
    this.ownerUserId = owner;
    this.columns = columns;
    this.creationDate = creationDate;
  }
}
