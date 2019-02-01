<template>
  <div class="container">
    <div class="py-5 text-center">
      <h2>Check missing key </h2>
      <p class="lead">From file [.vue,.cshtml] with file resources.</p>
    </div>
    <div class="row">
      <div class="col-md-4 mb-3">
        <label for="git_url">Project Name</label>
        <input type="text" class="form-control" v-model="gitUrl" placeholder="Input git url checking" value="" required="">
      </div>
      <div class="col-md-4">
        <select class="form-control">
          <option v-for="item in branches">Choice branch</option>
        </select>
      </div>
      <div>
        <div class="col-md-4">
          <br />
          <button type="button" class="btn btn-secondary" @click="getBranch">Get Branch</button>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-md-4">
        <br />
        <button type="button" class="btn btn-secondary" @click="submitCheck">GetProject</button>
      </div>
    </div>
    <div>
      <strong>Key Excess {{keyExcess}}</strong> <strong>Key Missing {{keyMissing}}</strong>
    </div>
    <div class="row">
      <h2>CSHTML({{cshtmlMissKeys.lenght}})</h2>
      <table class="table table-bordered">
        <thead>
          <tr>
            <th>Key</th>
            <th>Line</th>
            <th>Source</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in cshtmlMissKeys">
            <td>{{item.key}}</td>
            <td>{{item.line}}</td>
            <td>{{item.source}}</td>
          </tr>
        </tbody>
      </table>
    </div>
    <div class="row">
      <h2>Vue({{vueMissKeys.lenght}})</h2>
      <table class="table table-bordered">
        <thead>
          <tr>
            <th>Key</th>
            <th>Line</th>
            <th>Source</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in vueMissKeys">
            <td>{{item.key}}</td>
            <td>{{item.line}}</td>
            <td>{{item.source}}</td>
          </tr>
        </tbody>
      </table>
    </div>
    <div class="row">
      <h2>Resouce({{resouceLangs.lenght}})</h2>
      <table class="table table-bordered">
        <thead>
          <tr>
            <th>Key</th>
            <th>Value en-Us</th>
            <th>Used</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in resouceLangs">
            <td>{{item.key}}</td>
            <td>{{item.valueKey}}</td>
            <td>{{item.isUse}}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
<script>
  export default {
    data() {
      return {
        gitUrl: '',
        branches: [],
        vueMissKeys: [],
        cshtmlMissKeys: [],
        resouceLangs: [],
        keyExcess: '',
        keyMissing: '',

      }
    },
    methods: {
      getBranch: function () {
        debugger;
        this.$http.post('/api/get-branches?projectName=' + this.gitUrl).then(response => {
          console.log(response.data);
        }).catch((error) => console.log(error))
      },
      submitCheck: function () {
        this.$http.post('/api/analyzer?gitUrl=' + this.gitUrl).then(response => {
          console.log(response.data);
          this.vueMissKeys = response.data.result.vueMissKeys;
          this.cshtmlMissKeys = response.data.result.cshtmlMissKeys;
          this.resouceLangs = response.data.result.resouceLangs;
          this.resouceLangs = response.data.result.resouceLangs;
          this.keyExcess = response.data.result.keyExcess;
          this.keyMissing = response.data.result.keyMissing;
        }).catch((error) => console.log(error))
      },
    },
  }
</script>
<style>
</style>
