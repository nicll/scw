import { Injectable} from '@angular/core';
import {Apollo} from 'apollo-angular';
import {ApolloQueryResult} from '@apollo/client/core';
import { gql } from '@apollo/client/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApolloService {
  querySubscription: any;
  data:any;
  constructor(private apollo:Apollo) {
  }

  public GetData<T>(inquery:string):Observable<ApolloQueryResult<T>>{
    return this.apollo.watchQuery<T>({
      query: gql(inquery)
    })
      .valueChanges;
  }

  public QueryBuilder(table:string,...fields:string[]):string{
    let query=`query{${table}{`;
    fields.forEach(v=>{
      query=query.concat(`${v},`);
    })
    return query.slice(0,query.length-1).concat('}}');
  }
}
