# don't write the command to be executed by 'sh' to stdout
verbose(false)

MSBUILD    = 'C:/Windows/Microsoft.NET/Framework/v4.0.30319/msbuild.exe'
MONOLINKER = 'Tools/monolinker/monolinker.exe'
ILREPACK   = 'Tools/ILRepack/ILRepack.exe'

BUILD_DIR   = 'bin/build'
LINK_DIR    = 'bin/link'
MERGE_DIR   = 'bin/merge'
RELEASE_DIR = 'bin/release'

def tasks
  task :default => [:build, :link, :merge, :harvest]

  task :rebuild => [:clean, :default]

  desc 'Removes all build output'
  task :clean do
    puts 'Clean: Cleaning up previous build output'
    stopwatch 'Clean: Clean up' do
      rm_rf FileList['build/*']
      rm_rf FileList['src/**/bin']
      rm_rf FileList['src/**/obj']
    end
  end

  task :build do
    msbuild('Build', 'Release', 'src/QuickCheck.sln')
  end

  task :link do
    mkdir_p LINK_DIR

    link "#{LINK_DIR}/QuickCheck", BUILD_DIR, 'QuickCheck'
  end

  task :merge do
    mkdir_p MERGE_DIR

    merge "#{MERGE_DIR}/QuickCheck.dll",
      FileList["#{LINK_DIR}/QuickCheck/*.dll"]
  end

  task :harvest do
    mkdir_p RELEASE_DIR

    puts "Harvest: Copying final assemblies to #{RELEASE_DIR}"
    stopwatch('Harvest: Copying') do
      cp FileList["#{BUILD_DIR}/QuickCheck.*.dll"], RELEASE_DIR
      cp FileList["#{MERGE_DIR}/*"], RELEASE_DIR
    end
  end

  def stopwatch(operation='Operation')
    startTime = Time.now
    yield
    endTime = Time.now

    duration = endTime - startTime
    printf("#{operation} took %.1fs\n", duration)
  end

  def msbuild(target, config, project)
    cmd = "#{File.expand_path MSBUILD}"
    cmd << " #{project}"
    cmd << " /nologo"
    cmd << " /maxcpucount"
    cmd << " /verbosity:minimal"
    cmd << " /property:Configuration=#{config}"
    cmd << " /target:#{target}"

    puts "MSBuild: #{target} #{config} #{project}"
    stopwatch("MSBuild: #{target}") { sh cmd }
  end

  def link(target, input_dir, input_assembly)
    cmd = "#{File.expand_path MONOLINKER}"
    cmd << " -a #{input_assembly}"
    cmd << " -l none"
    cmd << " -o #{target}"
    cmd << " -d #{input_dir}"

    puts "MonoLinker: #{input_assembly} Optimization"
    stopwatch('MonoLinker: Optimization') do
      sh cmd
    end
  end

  def merge(target, inputs)
    cmd = "#{File.expand_path ILREPACK}"
    cmd << ' --internalize'
    cmd << ' --ndebug' # disable pdb files
    cmd << " --out:\"#{File.expand_path target}\""
    cmd << ' ' + inputs.map {|x| "\"#{x}\""}.join(' ')

    set_x86 = "#{File.expand_path CORFLAGS}"
    set_x86 << " /32bit+"
    set_x86 << " #{target}"
    set_x86 << " 1>NUL"

    puts "ILRepack: Merge Binaries -> #{target}"
    stopwatch('ILRepack: Merge') do
      sh cmd
      sh set_x86
    end
  end
end

tasks
