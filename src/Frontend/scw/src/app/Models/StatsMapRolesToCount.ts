export class StatsMapRolesToCount{
  role: string;
  counts: number[];
  constructor(role:string,counts:number[]) {
    this.role = role;
    this.counts = counts;
  }
}
