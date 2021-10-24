import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { ApolloQueryResult, FetchResult } from '@apollo/client/core';
import { gql } from '@apollo/client/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Apollo } from 'apollo-angular';
import { onError } from "@apollo/client/link/error";
@Injectable({
  providedIn: 'root'
})
export class ApolloService {
  constructor(private apollo: Apollo, private http: HttpClient) {
  }
  public GetData<T>(inquery: string): Observable<ApolloQueryResult<T>> {
    console.log(inquery);
    return this.apollo.watchQuery<T>({
      query: gql(inquery)
    }).valueChanges
  }
  public makeQueryRightCase(input: string): string {
    let firstchar = true;
    let result = "";
    const stringarr = Array.from(input);
    stringarr.forEach(v => {
      if (v.charCodeAt(0) < 58 && v.charCodeAt(0) > 47) {//Check if number than firstchar gets reset
        firstchar = true;
        result = result.concat(v);
        return;
      }
      if (firstchar) {
        result = result.concat(v.toUpperCase());
        firstchar = false;
      } else {
        result = result.concat(v);
      }
    });
    return result;
  }
  public QueryBuilder(table: string, fields: string[]): string {
    table = this.makeQueryRightCase(table + 's');
    console.log(table);
    let query = `query{all${table}{nodes{`;
    fields.forEach(v => {
      query = query.concat(`${v[0].toLowerCase()+v.substring(1)},`);
    })
    return query.slice(0, query.length - 1).concat('}}}');

  }
  public Insert(table: string, data: Map<string, string>): Observable<FetchResult<number, Record<string, any>, Record<string, any>>> {
    table = this.makeQueryRightCase(table);
    let mutation = `mutation {create${table}(`
    table=table[0].toLowerCase()+table.substring(1);
    if(table.charCodeAt(0)>47 && table.charCodeAt(0)<58){
      mutation=mutation+`input: {_${table}:{`;
    }
    else{
      mutation=mutation+`input: {${table}:{`;
    }
    data.forEach((key, val) => {
      mutation = mutation.concat(val + ":" + key + ",");
    });
    mutation = mutation.concat(`}}){__typename}}`);
    console.log(mutation);
    return this.apollo.mutate<number>({
      mutation: gql(mutation)
    })
  }
  public Update(table: string, id: number, data: Map<string, string>): Observable<FetchResult<number, Record<string, any>, Record<string, any>>> {
    table = this.makeQueryRightCase(table);
    let mutation="";
    mutation = `mutation {update${table}ById(`
    table=table[0].toLowerCase()+table.substring(1);
    if(table.charCodeAt(0)>47 && table.charCodeAt(0)<58){
      mutation=mutation+`input: { _id: ${id}, _${table}Patch:{`;
    }
    else{
      mutation=mutation+`input: { _id: ${id}, ${table}Patch:{`;
    }
    console.log(data)
    data.forEach((val:string, key:string) => {
      key=key[0].toLowerCase()+key.substring(1)
      //ToDo: parse
      val = String(val);
      console.log(val, key);
      if(val.includes(".")){
        if(val.indexOf(".") != -1 && val.indexOf(".") == val.lastIndexOf(".")) //Floats in graphql have to be parsed, everything else doesn't
        mutation = mutation.concat(key + `:` + parseFloat(val) + `,`);
      }
      else
        mutation = mutation.concat(key + `:"` + val + `",`);
    });
    mutation = mutation.concat(`}}){__typename}}`);
    console.log(mutation);
    return this.apollo.mutate<number>({
      mutation: gql(mutation)
    })
  }
  public Delete(table: string, id: number): Observable<FetchResult<number, Record<string, any>, Record<string, any>>> {
    table = this.makeQueryRightCase(table);
    let mutation = `mutation {delete${table}ById(`
    table=table[0].toLowerCase()+table.substring(1);
    mutation=mutation+`input: { id: ${id}`;
    mutation = mutation.concat(`}){__typename}}`);
    console.log(mutation)
    return this.apollo.mutate<number>({
      mutation: gql(mutation)
    })
  }
  public lookUpDataSetId(tableId: string): Observable<any> {
    return this.http.get(environment.aspUri+`/api/graphql/dataset/${tableId}/lookup`,
      { withCredentials: true, responseType: 'text' });
  }
  public lookUpSheetId(tableId: string): Observable<string> {
    return this.http.get(environment.aspUri+`/api/graphql/sheet/${tableId}/lookup`,
      { withCredentials: true, responseType: 'text' });
  }
}
