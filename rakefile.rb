# don't write the command to be executed by 'sh' to stdout
verbose(false)

MSBUILD = 'c:/windows/microsoft.net/framework/v4.0.30319/msbuild.exe'

def tasks
  task :default => [:copy_libs, :build, :release]

  task :rebuild => [:clean, :default]

  task :clean do
    puts 'clean: cleaning up previous build output'
    stopwatch 'clean: clean up' do
      rm_rf FileList['bin/*']
      rm_rf FileList['src/**/bin']
      rm_rf FileList['src/**/obj']
    end
  end

  task :copy_libs do
    puts 'copy: lib -> bin/build'
    stopwatch 'copy: copying' do
      mkdir_p 'bin/build'
      cp_r FileList['lib/*'], 'bin/build'
    end
  end

  task :build do
    msbuild 'release', 'src/QuickCheck.sln'
  end

  task :release do
    mkdir_p 'bin/release'

    files = FileList['bin/build/QuickCheck*.dll']

    puts 'release: copying final assemblies to bin/release'
    stopwatch('release: copying') do
      puts files
      cp files, 'bin/release'
    end
  end

  def stopwatch(operation='operation')
    startTime = Time.now
    yield
    endTime = Time.now

    duration = endTime - startTime
    printf("#{operation} took %.1fs\n", duration)
  end

  def msbuild(config, project)
    cmd = "#{File.expand_path MSBUILD}"
    cmd << " #{project}"
    cmd << " /nologo"
    cmd << " /maxcpucount"
    cmd << " /verbosity:minimal"
    cmd << " /property:configuration=#{config}"
    cmd << " /target:build"

    puts "msbuild: building #{project} (#{config} mode)"
    stopwatch 'msbuild: building' do
      sh cmd
    end
  end
end

tasks
